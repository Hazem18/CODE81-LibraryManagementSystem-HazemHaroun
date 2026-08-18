using Application.Features.Books.Commands.CreateBook;
using Application.Features.Books.Commands.DeleteBook;
using Application.Features.Books.Commands.UpdateBook;
using Application.Features.Books.Queries.GetBookById;
using Application.Features.Books.Queries.GetBooks;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = "ViewCatalog")]
    public async Task<IActionResult> GetBooks(
        [FromQuery] string? searchTerm,
        [FromQuery] int? categoryId,
        [FromQuery] BookStatus? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetBooksQuery(searchTerm, categoryId, status, pageNumber, pageSize));
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "ViewCatalog")]
    public async Task<IActionResult> GetBookById(int id)
    {
        var result = await _mediator.Send(new GetBookByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "ManageBooks")]
    public async Task<IActionResult> CreateBook(CreateBookCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBookById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "ManageBooks")]
    public async Task<IActionResult> UpdateBook(int id, UpdateBookCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id must match.");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "ManageBooks")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        await _mediator.Send(new DeleteBookCommand(id));
        return NoContent();
    }
}