using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Populated automatically by the SaveChanges interceptor in Infrastructure -
/// handlers never write to this table directly.
/// </summary>
public class ActivityLog : BaseEntity
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = string.Empty;     // Added / Modified / Deleted
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Details { get; set; }
}
