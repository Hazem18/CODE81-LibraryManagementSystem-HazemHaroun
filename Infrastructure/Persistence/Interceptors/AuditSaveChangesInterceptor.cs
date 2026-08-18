using System.Text.Json;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public AuditSaveChangesInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var auditEntries = new List<ActivityLog>();

        foreach (var entry in context.ChangeTracker.Entries().ToList())
        {
            // Never audit the audit table itself - this is the fix for the
            // "logging that you logged something" infinite-loop trap.
            if (entry.Entity is ActivityLog)
                continue;

            // Identity's own plumbing tables (roles, claims, tokens, logins) aren't
            // business data - skip everything under that namespace.
            if (entry.Entity.GetType().Namespace?.StartsWith("Microsoft.AspNetCore.Identity") == true)
                continue;

            if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
                continue;

            // Stamp Created/UpdatedAt automatically here, since this is the one
            // place that sees every Added/Modified entity - handlers never set
            // these themselves.
            if (entry.Entity is IAuditableEntity auditable)
            {
                if (entry.State == EntityState.Added)
                    auditable.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified)
                    auditable.UpdatedAt = DateTime.UtcNow;
            }

            var action = entry.State switch
            {
                EntityState.Added => "Added",
                EntityState.Modified => "Modified",
                EntityState.Deleted => "Deleted",
                _ => "Unknown"
            };

            var entityId = entry.Properties
                .FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString();

            var changedProps = entry.Properties
                .Where(p => entry.State == EntityState.Added || p.IsModified)
                .Where(p => !p.Metadata.IsPrimaryKey())
                .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);

            auditEntries.Add(new ActivityLog
            {
                UserId = _currentUserService.UserId,
                UserName = _currentUserService.UserName,
                Action = action,
                EntityName = entry.Entity.GetType().Name,
                EntityId = entityId,
                Timestamp = DateTime.UtcNow,
                Details = JsonSerializer.Serialize(changedProps)
            });
        }

        if (auditEntries.Count != 0)
            context.Set<ActivityLog>().AddRange(auditEntries);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
