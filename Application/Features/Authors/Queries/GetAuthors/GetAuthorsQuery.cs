using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Authors.Queries.GetAuthors;

public record GetAuthorsQuery(string? SearchTerm) : IRequest<List<AuthorDto>>;