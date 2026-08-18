using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Books.Specifications;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Books.Queries.GetBooks;

public class GetBooksQueryHandler : IRequestHandler<GetBooksQuery, PaginatedList<BookDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBooksQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<BookDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var spec = new BooksFilterSpec(request.SearchTerm, request.CategoryId, request.Status);
        var query = SpecificationEvaluator.Default.GetQuery(_context.Books.AsQueryable(), spec);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(b => new BookDto
            {
                Id = b.Id,
                ISBN = b.ISBN,
                Title = b.Title,
                Summary = b.Summary,
                Edition = b.Edition,
                PublicationYear = b.PublicationYear,
                Language = b.Language,
                CoverImageUrl = b.CoverImageUrl,
                Status = b.Status.ToString(),
                PublisherId = b.PublisherId,
                PublisherName = b.Publisher.Name,
                Authors = b.BookAuthors.Select(ba => ba.Author.FirstName + " " + ba.Author.LastName).ToList(),
                Categories = b.BookCategories.Select(bc => bc.Category.Name).ToList()
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<BookDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}