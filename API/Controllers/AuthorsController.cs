using Application.Features.Authors.Commands.CreateAuthor;
using Application.Features.Authors.Commands.DeleteAuthor;
using Application.Features.Authors.Commands.UpdateAuthor;
using Application.Features.Authors.Queries.GetAuthorById;
using Application.Features.Authors.Queries.GetAuthors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuthorsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthorsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "ViewCatalog")]
    public async Task<IActionResult> GetAuthors([FromQuery] string? searchTerm)
        => Ok(await _mediator.Send(new GetAuthorsQuery(searchTerm)));

    [HttpGet("{id}")]
    [Authorize(Policy = "ViewCatalog")]
    public async Task<IActionResult> GetAuthorById(int id)
        => Ok(await _mediator.Send(new GetAuthorByIdQuery(id)));

    [HttpPost]
    [Authorize(Policy = "ManageBooks")]
    public async Task<IActionResult> CreateAuthor(CreateAuthorCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAuthorById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "ManageBooks")]
    public async Task<IActionResult> UpdateAuthor(int id, UpdateAuthorCommand command)
    {
        if (id != command.Id) return BadRequest("Route id and body id must match.");
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "ManageBooks")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        await _mediator.Send(new DeleteAuthorCommand(id));
        return NoContent();
    }
}