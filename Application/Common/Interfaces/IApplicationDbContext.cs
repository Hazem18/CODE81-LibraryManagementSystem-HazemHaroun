using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    /// <summary>
    /// Application depends on this abstraction, not on EF Core's DbContext directly -
    /// Infrastructure provides the implementation. Keeps handlers testable without
    /// spinning up a real database.
    /// </summary>
    public interface IApplicationDbContext
    {
        DbSet<Book> Books { get; }
        DbSet<Author> Authors { get; }
        DbSet<BookAuthor> BookAuthors { get; }
        DbSet<Category> Categories { get; }
        DbSet<BookCategory> BookCategories { get; }
        DbSet<Publisher> Publishers { get; }
        DbSet<Member> Members { get; }
        DbSet<BorrowingTransaction> BorrowingTransactions { get; }
        DbSet<ActivityLog> ActivityLogs { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
