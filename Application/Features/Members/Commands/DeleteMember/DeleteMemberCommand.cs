using MediatR;

namespace Application.Features.Members.Commands.DeleteMember;

public record DeleteMemberCommand(int Id) : IRequest;
