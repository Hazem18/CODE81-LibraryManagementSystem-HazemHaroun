using Application.Features.Members.Commands.CreateMember;
using Application.Features.Members.Commands.DeleteMember;
using Application.Features.Members.Commands.UpdateMember;
using Application.Features.Members.Queries.GetMemberById;
using Application.Features.Members.Queries.GetMembers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly IMediator _mediator;
    public MembersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "ViewCatalog")]
    public async Task<IActionResult> GetMembers([FromQuery] string? searchTerm)
        => Ok(await _mediator.Send(new GetMembersQuery(searchTerm)));

    [HttpGet("{id}")]
    [Authorize(Policy = "ViewCatalog")]
    public async Task<IActionResult> GetMemberById(int id) => Ok(await _mediator.Send(new GetMemberByIdQuery(id)));

    [HttpPost]
    [Authorize(Policy = "ManageMembers")]
    public async Task<IActionResult> CreateMember(CreateMemberCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMemberById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "ManageMembers")]
    public async Task<IActionResult> UpdateMember(int id, UpdateMemberCommand command)
    {
        if (id != command.Id) return BadRequest("Route id and body id must match.");
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "ManageMembers")]
    public async Task<IActionResult> DeleteMember(int id)
    {
        await _mediator.Send(new DeleteMemberCommand(id));
        return NoContent();
    }
}
