using Application.Common.Dtos;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Members.Queries.GetMembers;

public class GetMembersQueryHandler : IRequestHandler<GetMembersQuery, List<MemberDto>>
{
    private readonly IApplicationDbContext _context;
    public GetMembersQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<MemberDto>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Members.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(m => m.FullName.Contains(request.SearchTerm) || m.Email.Contains(request.SearchTerm));

        return await query.OrderBy(m => m.FullName)
            .Select(m => new MemberDto
            {
                Id = m.Id,
                FullName = m.FullName,
                Email = m.Email,
                Phone = m.Phone,
                Address = m.Address,
                MembershipDate = m.MembershipDate,
                IsActive = m.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}
