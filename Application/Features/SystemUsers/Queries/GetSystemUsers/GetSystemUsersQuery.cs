using Application.Common.Dtos;
using MediatR;

namespace Application.Features.SystemUsers.Queries.GetSystemUsers;

public record GetSystemUsersQuery : IRequest<List<SystemUserDto>>;
