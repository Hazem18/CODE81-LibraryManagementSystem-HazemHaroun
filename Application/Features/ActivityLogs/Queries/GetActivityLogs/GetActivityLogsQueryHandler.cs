using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ActivityLogs.Queries.GetActivityLogs;

public class GetActivityLogsQueryHandler : IRequestHandler<GetActivityLogsQuery, PaginatedList<ActivityLogDto>>
{
    private readonly IApplicationDbContext _context;
    public GetActivityLogsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedList<ActivityLogDto>> Handle(GetActivityLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ActivityLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.EntityName))
            query = query.Where(a => a.EntityName == request.EntityName);

        if (!string.IsNullOrWhiteSpace(request.UserId))
            query = query.Where(a => a.UserId == request.UserId);

        query = query.OrderByDescending(a => a.Timestamp);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new ActivityLogDto
            {
                Id = a.Id,
                UserId = a.UserId,
                UserName = a.UserName,
                Action = a.Action,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                Timestamp = a.Timestamp,
                Details = a.Details
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<ActivityLogDto>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
