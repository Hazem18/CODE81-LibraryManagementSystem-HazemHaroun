using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Members.Queries.GetMembers;

public record GetMembersQuery(string? SearchTerm) : IRequest<List<MemberDto>>;
