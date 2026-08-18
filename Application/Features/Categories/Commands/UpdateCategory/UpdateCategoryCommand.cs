using MediatR;

namespace Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(int Id, string Name, string? Description, int? ParentCategoryId) : IRequest;
