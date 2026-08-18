using Application.Features.Borrowing.Commands.BorrowBook;
using Application.Features.Borrowing.Commands.ReturnBook;
using Application.Features.Borrowing.Queries.GetBorrowingHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BorrowingController : ControllerBase
{
    private readonly IMediator _mediator;
    public BorrowingController(IMediator mediator) => _mediator = mediator;

    [HttpPost("borrow")]
    [Authorize(Policy = "ProcessBorrowReturn")]
    public async Task<IActionResult> Borrow(BorrowBookCommand command) => Ok(await _mediator.Send(command));

    [HttpPost("{transactionId}/return")]
    [Authorize(Policy = "ProcessBorrowReturn")]
    public async Task<IActionResult> Return(int transactionId) => Ok(await _mediator.Send(new ReturnBookCommand(transactionId)));

    [HttpGet("history")]
    [Authorize(Policy = "ViewCatalog")]
    public async Task<IActionResult> GetHistory([FromQuery] int? memberId, [FromQuery] bool? overdueOnly)
        => Ok(await _mediator.Send(new GetBorrowingHistoryQuery(memberId, overdueOnly)));
}
