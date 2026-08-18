using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Books.Commands.CreateBook;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
{
    private readonly IApplicationDbContext _context;

    public CreateBookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var publisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Id == request.PublisherId, cancellationToken);
        if (publisher is null)
            throw new NotFoundException(nameof(Publisher), request.PublisherId);

        var book = new Book
        {
            ISBN = request.ISBN,
            Title = request.Title,
            Summary = request.Summary,
            Edition = request.Edition,
            PublicationYear = request.PublicationYear,
            Language = request.Language,
            CoverImageUrl = request.CoverImageUrl,
            PublisherId = request.PublisherId,
            Status = BookStatus.Available
        };

        foreach (var authorId in request.AuthorIds.Distinct())
            book.BookAuthors.Add(new BookAuthor { AuthorId = authorId });

        foreach (var categoryId in request.CategoryIds.Distinct())
            book.BookCategories.Add(new BookCategory { CategoryId = categoryId });

        _context.Books.Add(book);
        await _context.SaveChangesAsync(cancellationToken);

        var authors = await _context.Authors.Where(a => request.AuthorIds.Contains(a.Id)).ToListAsync(cancellationToken);
        var categories = await _context.Categories.Where(c => request.CategoryIds.Contains(c.Id)).ToListAsync(cancellationToken);

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
            PublisherName = publisher.Name,
            Authors = authors.Select(a => $"{a.FirstName} {a.LastName}").ToList(),
            Categories = categories.Select(c => c.Name).ToList()
        };
    }
}