using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.Property(b => b.ISBN).IsRequired().HasMaxLength(20);
        builder.HasIndex(b => b.ISBN).IsUnique();

        builder.Property(b => b.Title).IsRequired().HasMaxLength(300);
        builder.Property(b => b.Edition).HasMaxLength(50);
        builder.Property(b => b.Language).HasMaxLength(50);
        builder.Property(b => b.Summary).HasMaxLength(2000);

        builder.HasOne(b => b.Publisher)
            .WithMany(p => p.Books)
            .HasForeignKey(b => b.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
