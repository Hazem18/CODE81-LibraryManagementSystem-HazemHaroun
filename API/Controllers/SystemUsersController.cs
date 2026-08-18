using Application.Features.SystemUsers.Commands.CreateSystemUser;
using Application.Features.SystemUsers.Commands.DeactivateSystemUser;
using Application.Features.SystemUsers.Commands.UpdateSystemUser;
using Application.Features.SystemUsers.Queries.GetSystemUserById;
using Application.Features.SystemUsers.Queries.GetSystemUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// Every action here is Administrator-only - managing system user accounts
// (including just viewing the list) is not something Librarian/Staff should
// see, unlike Books/Members which Staff can at least read.
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "ManageSystemUsers")]
public class SystemUsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public SystemUsersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetSystemUsers() => Ok(await _mediator.Send(new GetSystemUsersQuery()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSystemUserById(string id) => Ok(await _mediator.Send(new GetSystemUserByIdQuery(id)));

    [HttpPost]
    public async Task<IActionResult> CreateSystemUser(CreateSystemUserCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSystemUserById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSystemUser(string id, UpdateSystemUserCommand command)
    {
        if (id != command.Id) return BadRequest("Route id and body id must match.");
        await _mediator.Send(command);
        return NoContent();
    }

    // Deactivates rather than hard-deletes - see the comment in
    // DeactivateSystemUserCommandHandler for why (BorrowingTransaction FK).
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeactivateSystemUser(string id)
    {
        await _mediator.Send(new DeactivateSystemUserCommand(id));
        return NoContent();
    }
}
