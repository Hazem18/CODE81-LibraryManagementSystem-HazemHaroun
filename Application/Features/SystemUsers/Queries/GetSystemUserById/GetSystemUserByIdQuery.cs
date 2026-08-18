using Application.Common.Dtos;
using MediatR;

namespace Application.Features.SystemUsers.Queries.GetSystemUserById;

public record GetSystemUserByIdQuery(string Id) : IRequest<SystemUserDto>;
