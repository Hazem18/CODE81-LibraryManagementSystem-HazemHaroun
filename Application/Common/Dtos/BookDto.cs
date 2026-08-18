namespace Application.Common.Dtos;

public class BookDto
{
    public int Id { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Edition { get; set; }
    public int? PublicationYear { get; set; }
    public string? Language { get; set; }
    public string? CoverImageUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public int PublisherId { get; set; }
    public string PublisherName { get; set; } = string.Empty;
    public List<string> Authors { get; set; } = new();
    public List<string> Categories { get; set; } = new();
}