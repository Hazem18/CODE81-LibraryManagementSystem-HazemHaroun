using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Publishers.Commands.UpdatePublisher;

public class UpdatePublisherCommandHandler : IRequestHandler<UpdatePublisherCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdatePublisherCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdatePublisherCommand request, CancellationToken cancellationToken)
    {
        var publisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (publisher is null) throw new NotFoundException(nameof(Publisher), request.Id);

        publisher.Name = request.Name;
        publisher.Address = request.Address;
        publisher.ContactEmail = request.ContactEmail;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
