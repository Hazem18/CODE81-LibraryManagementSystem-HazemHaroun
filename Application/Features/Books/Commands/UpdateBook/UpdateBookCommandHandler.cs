using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Books.Commands.UpdateBook;

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateBookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _context.Books
            .Include(b => b.BookAuthors)
            .Include(b => b.BookCategories)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (book is null)
            throw new NotFoundException(nameof(Book), request.Id);

        var publisherExists = await _context.Publishers.AnyAsync(p => p.Id == request.PublisherId, cancellationToken);
        if (!publisherExists)
            throw new NotFoundException(nameof(Publisher), request.PublisherId);

        book.ISBN = request.ISBN;
        book.Title = request.Title;
        book.Summary = request.Summary;
        book.Edition = request.Edition;
        book.PublicationYear = request.PublicationYear;
        book.Language = request.Language;
        book.CoverImageUrl = request.CoverImageUrl;
        book.PublisherId = request.PublisherId;

        // Simplest correct way to sync a many-to-many on update: clear and re-add,
        // rather than diffing old vs new lists. A book has a handful of authors/
        // categories at most, so clarity wins over the extra efficiency of diffing.
        book.BookAuthors.Clear();
        foreach (var authorId in request.AuthorIds.Distinct())
            book.BookAuthors.Add(new BookAuthor { AuthorId = authorId, BookId = book.Id });

        book.BookCategories.Clear();
        foreach (var categoryId in request.CategoryIds.Distinct())
            book.BookCategories.Add(new BookCategory { CategoryId = categoryId, BookId = book.Id });

        await _context.SaveChangesAsync(cancellationToken);
    }
}