using Application.Features.ActivityLogs.Queries.GetActivityLogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "ViewActivityLogs")]
public class ActivityLogsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ActivityLogsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetActivityLogs(
        [FromQuery] string? entityName,
        [FromQuery] string? userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
        => Ok(await _mediator.Send(new GetActivityLogsQuery(entityName, userId, pageNumber, pageSize)));
}
