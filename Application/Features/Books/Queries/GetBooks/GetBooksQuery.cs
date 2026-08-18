using Application.Common.Dtos;
using Application.Common.Models;
using Domain.Enums;
using MediatR;

namespace Application.Features.Books.Queries.GetBooks;

public record GetBooksQuery(
    string? SearchTerm,
    int? CategoryId,
    BookStatus? Status,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<PaginatedList<BookDto>>;