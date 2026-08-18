using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Publishers.Commands.DeletePublisher;

public class DeletePublisherCommandHandler : IRequestHandler<DeletePublisherCommand>
{
    private readonly IApplicationDbContext _context;
    public DeletePublisherCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeletePublisherCommand request, CancellationToken cancellationToken)
    {
        var publisher = await _context.Publishers
            .Include(p => p.Books)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (publisher is null) throw new NotFoundException(nameof(Publisher), request.Id);

        if (publisher.Books.Count != 0)
            throw new BusinessRuleException("Cannot delete a publisher linked to existing books.");

        _context.Publishers.Remove(publisher);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
