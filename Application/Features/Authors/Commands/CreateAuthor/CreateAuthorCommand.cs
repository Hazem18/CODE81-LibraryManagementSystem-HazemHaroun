using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Authors.Commands.CreateAuthor;

public record CreateAuthorCommand(string FirstName, string LastName, string? Bio) : IRequest<AuthorDto>;