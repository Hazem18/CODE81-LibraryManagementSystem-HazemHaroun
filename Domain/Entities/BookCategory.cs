namespace Domain.Entities;

/// <summary> Join entity: many-to-many Book <-> Category. </summary>
public class BookCategory
{
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
