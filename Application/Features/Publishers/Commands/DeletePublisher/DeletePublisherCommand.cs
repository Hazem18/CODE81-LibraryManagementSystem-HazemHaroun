using MediatR;

namespace Application.Features.Publishers.Commands.DeletePublisher;

public record DeletePublisherCommand(int Id) : IRequest;
