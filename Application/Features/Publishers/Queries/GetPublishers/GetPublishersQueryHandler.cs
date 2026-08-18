using Application.Common.Dtos;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Publishers.Queries.GetPublishers;

public class GetPublishersQueryHandler : IRequestHandler<GetPublishersQuery, List<PublisherDto>>
{
    private readonly IApplicationDbContext _context;
    public GetPublishersQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<PublisherDto>> Handle(GetPublishersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Publishers
            .OrderBy(p => p.Name)
            .Select(p => new PublisherDto { Id = p.Id, Name = p.Name, Address = p.Address, ContactEmail = p.ContactEmail })
            .ToListAsync(cancellationToken);
    }
}
