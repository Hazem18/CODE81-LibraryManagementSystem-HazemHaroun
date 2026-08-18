using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Members.Queries.GetMemberById;

public class GetMemberByIdQueryHandler : IRequestHandler<GetMemberByIdQuery, MemberDto>
{
    private readonly IApplicationDbContext _context;
    public GetMemberByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<MemberDto> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if (member is null) throw new NotFoundException(nameof(Member), request.Id);

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
