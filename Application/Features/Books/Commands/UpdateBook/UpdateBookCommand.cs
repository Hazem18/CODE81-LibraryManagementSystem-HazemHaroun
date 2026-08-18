using MediatR;

namespace Application.Features.Books.Commands.UpdateBook;

public record UpdateBookCommand(
    int Id,
    string ISBN,
    string Title,
    string? Summary,
    string? Edition,
    int? PublicationYear,
    string? Language,
    string? CoverImageUrl,
    int PublisherId,
    List<int> AuthorIds,
    List<int> CategoryIds) : IRequest;