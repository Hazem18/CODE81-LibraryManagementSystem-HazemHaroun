using MediatR;

namespace Application.Features.SystemUsers.Commands.DeactivateSystemUser;

public record DeactivateSystemUserCommand(string Id) : IRequest;
