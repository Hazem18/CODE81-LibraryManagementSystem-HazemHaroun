using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Books.Queries.GetBookById;

public record GetBookByIdQuery(int Id) : IRequest<BookDto>;