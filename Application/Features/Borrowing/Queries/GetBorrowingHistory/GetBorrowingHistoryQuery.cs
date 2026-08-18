using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Borrowing.Queries.GetBorrowingHistory;

public record GetBorrowingHistoryQuery(int? MemberId, bool? OverdueOnly) : IRequest<List<BorrowingTransactionDto>>;
