using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Books.Commands.CreateBook;

public record CreateBookCommand(
    string ISBN,
    string Title,
    string? Summary,
    string? Edition,
    int? PublicationYear,
    string? Language,
    string? CoverImageUrl,
    int PublisherId,
    List<int> AuthorIds,
    List<int> CategoryIds) : IRequest<BookDto>;