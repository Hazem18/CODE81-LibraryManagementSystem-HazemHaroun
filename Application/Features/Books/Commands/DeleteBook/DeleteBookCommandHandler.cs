using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Books.Commands.DeleteBook;

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteBookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (book is null)
            throw new NotFoundException(nameof(Book), request.Id);

        var hasActiveBorrowings = await _context.BorrowingTransactions
            .AnyAsync(t => t.BookId == request.Id && t.Status == BorrowingStatus.Borrowed, cancellationToken);

        if (hasActiveBorrowings)
            throw new BusinessRuleException("Cannot delete a book that is currently borrowed.");

        _context.Books.Remove(book);
        await _context.SaveChangesAsync(cancellationToken);
    }
}