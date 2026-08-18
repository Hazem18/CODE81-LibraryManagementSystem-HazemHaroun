using MediatR;

namespace Application.Features.Authors.Commands.UpdateAuthor;

public record UpdateAuthorCommand(int Id, string FirstName, string LastName, string? Bio) : IRequest;