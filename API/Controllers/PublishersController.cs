using Application.Features.Publishers.Commands.CreatePublisher;
using Application.Features.Publishers.Commands.DeletePublisher;
using Application.Features.Publishers.Commands.UpdatePublisher;
using Application.Features.Publishers.Queries.GetPublisherById;
using Application.Features.Publishers.Queries.GetPublishers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PublishersController : ControllerBase
{
    private readonly IMediator _mediator;
    public PublishersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "ViewCatalog")]
    public async Task<IActionResult> GetPublishers() => Ok(await _mediator.Send(new GetPublishersQuery()));

    [HttpGet("{id}")]
    [Authorize(Policy = "ViewCatalog")]
    public async Task<IActionResult> GetPublisherById(int id) => Ok(await _mediator.Send(new GetPublisherByIdQuery(id)));

    [HttpPost]
    [Authorize(Policy = "ManageBooks")]
    public async Task<IActionResult> CreatePublisher(CreatePublisherCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPublisherById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "ManageBooks")]
    public async Task<IActionResult> UpdatePublisher(int id, UpdatePublisherCommand command)
    {
        if (id != command.Id) return BadRequest("Route id and body id must match.");
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "ManageBooks")]
    public async Task<IActionResult> DeletePublisher(int id)
    {
        await _mediator.Send(new DeletePublisherCommand(id));
        return NoContent();
    }
}
