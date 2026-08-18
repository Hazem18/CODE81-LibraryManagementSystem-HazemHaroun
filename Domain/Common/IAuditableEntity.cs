namespace Domain.Common
{
    /// <summary>
    /// Implemented by entities whose Created/Updated timestamps are stamped
    /// automatically by the SaveChanges interceptor in Infrastructure, rather
    /// than being set manually in command handlers.
    /// </summary>
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}
