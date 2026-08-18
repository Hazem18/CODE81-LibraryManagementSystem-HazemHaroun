using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Publishers.Queries.GetPublisherById;

public class GetPublisherByIdQueryHandler : IRequestHandler<GetPublisherByIdQuery, PublisherDto>
{
    private readonly IApplicationDbContext _context;
    public GetPublisherByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PublisherDto> Handle(GetPublisherByIdQuery request, CancellationToken cancellationToken)
    {
        var publisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (publisher is null) throw new NotFoundException(nameof(Publisher), request.Id);

        return new PublisherDto { Id = publisher.Id, Name = publisher.Name, Address = publisher.Address, ContactEmail = publisher.ContactEmail };
    }
}
