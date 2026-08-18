using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BorrowingTransactionConfiguration : IEntityTypeConfiguration<BorrowingTransaction>
{
    public void Configure(EntityTypeBuilder<BorrowingTransaction> builder)
    {
        // All Restrict: Book, Member and ApplicationUser are each referenced from
        // multiple places once you add more relations later, so SQL Server would
        // reject Cascade here too ("multiple cascade paths"). Returns/deletes are
        // explicit application actions, not implicit cascades, which is the right
        // call for financial/audit-adjacent data like borrowing history anyway.
        builder.HasOne(t => t.Book)
            .WithMany(b => b.BorrowingTransactions)
            .HasForeignKey(t => t.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Member)
            .WithMany(m => m.BorrowingTransactions)
            .HasForeignKey(t => t.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.IssuedByUser)
            .WithMany(u => u.ProcessedTransactions)
            .HasForeignKey(t => t.IssuedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
