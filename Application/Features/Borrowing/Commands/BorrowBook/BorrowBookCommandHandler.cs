using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Borrowing.Commands.BorrowBook;

public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand, BorrowingTransactionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public BorrowBookCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<BorrowingTransactionDto> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == request.BookId, cancellationToken);
        if (book is null) throw new NotFoundException(nameof(Book), request.BookId);

        if (book.Status != BookStatus.Available)
            throw new BusinessRuleException($"Book '{book.Title}' is not available for borrowing (current status: {book.Status}).");

        var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == request.MemberId, cancellationToken);
        if (member is null) throw new NotFoundException(nameof(Member), request.MemberId);

        if (!member.IsActive)
            throw new BusinessRuleException("Cannot lend a book to an inactive member.");

        var currentUserId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("Unable to determine the current user from the token.");

        var transaction = new BorrowingTransaction
        {
            BookId = book.Id,
            MemberId = member.Id,
            IssuedByUserId = currentUserId,
            BorrowDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14), // Fixed 14-day loan period - renewal is a known future improvement, not in scope.
            Status = BorrowingStatus.Borrowed
        };

        book.Status = BookStatus.Borrowed;

        _context.BorrowingTransactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        var issuedByUser = await _userManager.FindByIdAsync(currentUserId);

        return new BorrowingTransactionDto
        {
            Id = transaction.Id,
            BookId = book.Id,
            BookTitle = book.Title,
            MemberId = member.Id,
            MemberName = member.FullName,
            IssuedByUserId = currentUserId,
            IssuedByUserName = issuedByUser?.FullName ?? string.Empty,
            BorrowDate = transaction.BorrowDate,
            DueDate = transaction.DueDate,
            ReturnDate = transaction.ReturnDate,
            Status = transaction.Status.ToString()
        };
    }
}
