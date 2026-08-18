using Application.Common.Dtos;
using MediatR;

namespace Application.Features.SystemUsers.Commands.CreateSystemUser;

public record CreateSystemUserCommand(string FullName, string Email, string Password, string Role) : IRequest<SystemUserDto>;
