namespace Domain.Entities;

/// <summary> Join entity: many-to-many Book <-> Author. </summary>
public class BookAuthor
{
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
}
