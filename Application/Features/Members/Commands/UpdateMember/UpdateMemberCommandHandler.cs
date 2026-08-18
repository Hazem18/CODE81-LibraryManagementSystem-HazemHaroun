using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Members.Commands.UpdateMember;

public class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateMemberCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if (member is null) throw new NotFoundException(nameof(Member), request.Id);

        var emailTakenByAnother = await _context.Members
            .AnyAsync(m => m.Email == request.Email && m.Id != request.Id, cancellationToken);
        if (emailTakenByAnother)
            throw new BusinessRuleException("Another member is already using this email.");

        member.FullName = request.FullName;
        member.Email = request.Email;
        member.Phone = request.Phone;
        member.Address = request.Address;
        member.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
