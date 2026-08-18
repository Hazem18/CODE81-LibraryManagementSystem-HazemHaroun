using Domain.Common;

namespace Domain.Entities;

public class Publisher : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? ContactEmail { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
