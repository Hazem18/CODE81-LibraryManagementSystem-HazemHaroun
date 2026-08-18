using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Publishers.Commands.CreatePublisher;

public record CreatePublisherCommand(string Name, string? Address, string? ContactEmail) : IRequest<PublisherDto>;
