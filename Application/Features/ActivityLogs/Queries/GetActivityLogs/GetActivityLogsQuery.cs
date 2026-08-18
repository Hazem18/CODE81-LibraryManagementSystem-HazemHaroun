using Application.Common.Dtos;
using Application.Common.Models;
using MediatR;

namespace Application.Features.ActivityLogs.Queries.GetActivityLogs;

public record GetActivityLogsQuery(
    string? EntityName,
    string? UserId,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PaginatedList<ActivityLogDto>>;
