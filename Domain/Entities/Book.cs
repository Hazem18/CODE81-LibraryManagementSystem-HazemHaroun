using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Book : BaseEntity, IAuditableEntity
{
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Edition { get; set; }
    public int? PublicationYear { get; set; }
    public string? Language { get; set; }
    public string? CoverImageUrl { get; set; }
    public BookStatus Status { get; set; } = BookStatus.Available;

    public int PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    public ICollection<BorrowingTransaction> BorrowingTransactions { get; set; } = new List<BorrowingTransaction>();

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
