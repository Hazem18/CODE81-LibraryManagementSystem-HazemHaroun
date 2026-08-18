using Domain.Common;

namespace Domain.Entities;

public class Author : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Bio { get; set; }

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

    public string FullName => $"{FirstName} {LastName}";
}
