using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Borrowing.Commands.ReturnBook;

public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, BorrowingTransactionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReturnBookCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<BorrowingTransactionDto> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.BorrowingTransactions
            .Include(t => t.Book)
            .Include(t => t.Member)
            .FirstOrDefaultAsync(t => t.Id == request.TransactionId, cancellationToken);

        if (transaction is null) throw new NotFoundException(nameof(BorrowingTransaction), request.TransactionId);

        if (transaction.Status is not (BorrowingStatus.Borrowed or BorrowingStatus.Overdue))
            throw new BusinessRuleException("This transaction has already been returned.");

        transaction.ReturnDate = DateTime.UtcNow;
        transaction.Status = BorrowingStatus.Returned;
        transaction.Book.Status = BookStatus.Available;

        await _context.SaveChangesAsync(cancellationToken);

        var issuedByUser = await _userManager.FindByIdAsync(transaction.IssuedByUserId);

        return new BorrowingTransactionDto
        {
            Id = transaction.Id,
            BookId = transaction.BookId,
            BookTitle = transaction.Book.Title,
            MemberId = transaction.MemberId,
            MemberName = transaction.Member.FullName,
            IssuedByUserId = transaction.IssuedByUserId,
            IssuedByUserName = issuedByUser?.FullName ?? string.Empty,
            BorrowDate = transaction.BorrowDate,
            DueDate = transaction.DueDate,
            ReturnDate = transaction.ReturnDate,
            Status = transaction.Status.ToString()
        };
    }
}
