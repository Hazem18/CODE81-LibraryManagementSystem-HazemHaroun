using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string? Description, int? ParentCategoryId) : IRequest<CategoryDto>;
