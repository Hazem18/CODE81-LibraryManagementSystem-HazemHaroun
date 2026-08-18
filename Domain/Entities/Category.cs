using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Self-referencing so genres can nest (e.g. Fiction -> Sci-Fi -> Space Opera),
/// per the "hierarchical structure" requirement in the task doc.
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();

    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
}
