using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Borrowing.Commands.BorrowBook;

public record BorrowBookCommand(int BookId, int MemberId) : IRequest<BorrowingTransactionDto>;
