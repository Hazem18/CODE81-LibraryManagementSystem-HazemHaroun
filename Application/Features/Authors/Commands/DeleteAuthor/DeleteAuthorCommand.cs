using MediatR;

namespace Application.Features.Authors.Commands.DeleteAuthor;

public record DeleteAuthorCommand(int Id) : IRequest;