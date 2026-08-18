using Application.Features.Categories.Commands.CreateCategory;
using Application.Features.Categories.Commands.DeleteCategory;
using Application.Features.Categories.Commands.UpdateCategory;
using Application.Features.Categories.Queries.GetCategories;
using Application.Features.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    public CategoriesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "ViewCatalog")]
    public async Task<IActionResult> GetCategories() => Ok(await _mediator.Send(new GetCategoriesQuery()));

    [HttpGet("{id}")]
    [Authorize(Policy = "ViewCatalog")]
    public async Task<IActionResult> GetCategoryById(int id) => Ok(await _mediator.Send(new GetCategoryByIdQuery(id)));

    [HttpPost]
    [Authorize(Policy = "ManageBooks")]
    public async Task<IActionResult> CreateCategory(CreateCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "ManageBooks")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryCommand command)
    {
        if (id != command.Id) return BadRequest("Route id and body id must match.");
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "ManageBooks")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _mediator.Send(new DeleteCategoryCommand(id));
        return NoContent();
    }
}
