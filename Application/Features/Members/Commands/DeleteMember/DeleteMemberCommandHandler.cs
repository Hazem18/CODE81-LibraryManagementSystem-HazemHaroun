using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Members.Commands.DeleteMember;

public class DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteMemberCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _context.Members
            .Include(m => m.BorrowingTransactions)
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (member is null) throw new NotFoundException(nameof(Member), request.Id);

        var hasActiveBorrowings = member.BorrowingTransactions.Any(t => t.Status == BorrowingStatus.Borrowed);
        if (hasActiveBorrowings)
            throw new BusinessRuleException("Cannot delete a member with active borrowings.");

        _context.Members.Remove(member);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
