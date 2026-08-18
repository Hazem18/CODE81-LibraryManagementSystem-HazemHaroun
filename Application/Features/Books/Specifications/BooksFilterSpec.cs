using Ardalis.Specification;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.Books.Specifications;

public class BooksFilterSpec : Specification<Book>
{
    public BooksFilterSpec(string? searchTerm, int? categoryId, BookStatus? status)
    {
        Query.Include(b => b.Publisher)
             .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
             .Include(b => b.BookCategories).ThenInclude(bc => bc.Category);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            Query.Where(b =>
                b.Title.Contains(searchTerm) ||
                b.BookAuthors.Any(ba => ba.Author.FirstName.Contains(searchTerm) || ba.Author.LastName.Contains(searchTerm)));
        }

        if (categoryId.HasValue)
        {
            Query.Where(b => b.BookCategories.Any(bc => bc.CategoryId == categoryId.Value));
        }

        if (status.HasValue)
        {
            Query.Where(b => b.Status == status.Value);
        }

        Query.OrderBy(b => b.Title);
    }
}