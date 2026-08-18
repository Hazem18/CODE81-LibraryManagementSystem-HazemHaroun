using MediatR;

namespace Application.Features.Books.Commands.DeleteBook;

public record DeleteBookCommand(int Id) : IRequest;