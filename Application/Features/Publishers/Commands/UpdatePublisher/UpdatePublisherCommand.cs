using MediatR;

namespace Application.Features.Publishers.Commands.UpdatePublisher;

public record UpdatePublisherCommand(int Id, string Name, string? Address, string? ContactEmail) : IRequest;
