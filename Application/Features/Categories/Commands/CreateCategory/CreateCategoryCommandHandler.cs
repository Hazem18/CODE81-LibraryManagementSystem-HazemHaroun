using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IApplicationDbContext _context;
    public CreateCategoryCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        Category? parent = null;

        if (request.ParentCategoryId.HasValue)
        {
            parent = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId.Value, cancellationToken);
            if (parent is null)
                throw new NotFoundException(nameof(Category), request.ParentCategoryId.Value);
        }

        var category = new Category { Name = request.Name, Description = request.Description, ParentCategoryId = request.ParentCategoryId };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            ParentCategoryName = parent?.Name
        };
    }
}
