using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Publishers.Queries.GetPublishers;

public record GetPublishersQuery : IRequest<List<PublisherDto>>;
