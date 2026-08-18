using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Members.Queries.GetMemberById;

public record GetMemberByIdQuery(int Id) : IRequest<MemberDto>;
