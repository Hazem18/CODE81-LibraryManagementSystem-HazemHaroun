using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteCategoryCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(c => c.SubCategories)
            .Include(c => c.BookCategories)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null) throw new NotFoundException(nameof(Category), request.Id);

        if (category.SubCategories.Count != 0)
            throw new BusinessRuleException("Cannot delete a category that has subcategories. Delete or reassign them first.");

        if (category.BookCategories.Count != 0)
            throw new BusinessRuleException("Cannot delete a category assigned to existing books.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
