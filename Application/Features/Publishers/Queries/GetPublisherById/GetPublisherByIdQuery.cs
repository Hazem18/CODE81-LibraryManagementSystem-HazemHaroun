using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Publishers.Queries.GetPublisherById;

public record GetPublisherByIdQuery(int Id) : IRequest<PublisherDto>;
