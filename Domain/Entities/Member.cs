using Domain.Common;

namespace Domain.Entities;

public class Member : BaseEntity, IAuditableEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime MembershipDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<BorrowingTransaction> BorrowingTransactions { get; set; } = new List<BorrowingTransaction>();

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
