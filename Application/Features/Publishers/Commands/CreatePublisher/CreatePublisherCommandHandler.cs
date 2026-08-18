using Application.Common.Dtos;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Publishers.Commands.CreatePublisher;

public class CreatePublisherCommandHandler : IRequestHandler<CreatePublisherCommand, PublisherDto>
{
    private readonly IApplicationDbContext _context;
    public CreatePublisherCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<PublisherDto> Handle(CreatePublisherCommand request, CancellationToken cancellationToken)
    {
        var publisher = new Publisher { Name = request.Name, Address = request.Address, ContactEmail = request.ContactEmail };
        _context.Publishers.Add(publisher);
        await _context.SaveChangesAsync(cancellationToken);

        return new PublisherDto { Id = publisher.Id, Name = publisher.Name, Address = publisher.Address, ContactEmail = publisher.ContactEmail };
    }
}
