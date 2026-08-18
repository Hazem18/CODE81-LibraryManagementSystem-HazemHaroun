using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Borrowing.Commands.ReturnBook;

public record ReturnBookCommand(int TransactionId) : IRequest<BorrowingTransactionDto>;
