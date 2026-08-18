using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Books.Queries.GetBookById;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto>
{
    private readonly IApplicationDbContext _context;

    public GetBookByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BookDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .Include(b => b.Publisher)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (book is null)
            throw new NotFoundException(nameof(Book), request.Id);

        return new BookDto
        {
            Id = book.Id,
            ISBN = book.ISBN,
            Title = book.Title,
            Summary = book.Summary,
            Edition = book.Edition,
            PublicationYear = book.PublicationYear,
            Language = book.Language,
            CoverImageUrl = book.CoverImageUrl,
            Status = book.Status.ToString(),
            PublisherId = book.PublisherId,
            PublisherName = book.Publisher.Name,
            Authors = book.BookAuthors.Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}").ToList(),
            Categories = book.BookCategories.Select(bc => bc.Category.Name).ToList()
        };
    }
}