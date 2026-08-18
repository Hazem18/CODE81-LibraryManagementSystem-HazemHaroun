using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class BorrowingTransaction : BaseEntity
{
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;

    /// <summary> The staff/librarian who processed this borrow or return. </summary>
    public string IssuedByUserId { get; set; } = string.Empty;
    public ApplicationUser IssuedByUser { get; set; } = null!;

    public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public BorrowingStatus Status { get; set; } = BorrowingStatus.Borrowed;
    public string? Notes { get; set; }
}
