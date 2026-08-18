using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Members.Commands.CreateMember;

public record CreateMemberCommand(string FullName, string Email, string? Phone, string? Address) : IRequest<MemberDto>;
