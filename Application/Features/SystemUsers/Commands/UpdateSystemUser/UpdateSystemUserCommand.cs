using MediatR;

namespace Application.Features.SystemUsers.Commands.UpdateSystemUser;

public record UpdateSystemUserCommand(string Id, string FullName, bool IsActive, string Role) : IRequest;
