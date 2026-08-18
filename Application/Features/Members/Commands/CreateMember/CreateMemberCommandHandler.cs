using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Members.Commands.CreateMember;

public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, MemberDto>
{
    private readonly IApplicationDbContext _context;
    public CreateMemberCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<MemberDto> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _context.Members.AnyAsync(m => m.Email == request.Email, cancellationToken);
        if (emailExists)
            throw new BusinessRuleException("A member with this email already exists.");

        var member = new Member
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            MembershipDate = DateTime.UtcNow,
            IsActive = true
        };

        _context.Members.Add(member);
        await _context.SaveChangesAsync(cancellationToken);

        return new MemberDto
        {
            Id = member.Id,
            FullName = member.FullName,
            Email = member.Email,
            Phone = member.Phone,
            Address = member.Address,
            MembershipDate = member.MembershipDate,
            IsActive = member.IsActive
        };
    }
}
