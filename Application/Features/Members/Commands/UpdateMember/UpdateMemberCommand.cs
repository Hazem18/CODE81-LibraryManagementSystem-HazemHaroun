using MediatR;

namespace Application.Features.Members.Commands.UpdateMember;

public record UpdateMemberCommand(int Id, string FullName, string Email, string? Phone, string? Address, bool IsActive) : IRequest;
