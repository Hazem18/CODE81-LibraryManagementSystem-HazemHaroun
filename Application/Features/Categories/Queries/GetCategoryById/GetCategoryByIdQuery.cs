using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto>;
