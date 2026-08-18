using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateCategoryCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (category is null) throw new NotFoundException(nameof(Category), request.Id);

        if (request.ParentCategoryId.HasValue)
        {
            if (request.ParentCategoryId.Value == request.Id)
                throw new BusinessRuleException("A category cannot be its own parent.");

            var parentExists = await _context.Categories.AnyAsync(c => c.Id == request.ParentCategoryId.Value, cancellationToken);
            if (!parentExists)
                throw new NotFoundException(nameof(Category), request.ParentCategoryId.Value);

            // Walk UP from the proposed new parent through its own ancestor chain.
            // If we ever reach this category's own Id, the proposed parent is
            // actually one of THIS category's descendants - assigning it would
            // create a cycle (e.g. Fiction -> SciFi -> SpaceOpera -> Fiction).
            var currentId = request.ParentCategoryId;
            while (currentId.HasValue)
            {
                if (currentId.Value == request.Id)
                    throw new BusinessRuleException("This change would create a circular category hierarchy.");

                currentId = await _context.Categories
                    .Where(c => c.Id == currentId.Value)
                    .Select(c => c.ParentCategoryId)
                    .FirstOrDefaultAsync(cancellationToken);
            }
        }

        category.Name = request.Name;
        category.Description = request.Description;
        category.ParentCategoryId = request.ParentCategoryId;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
