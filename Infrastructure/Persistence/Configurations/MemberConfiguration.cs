using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.Property(m => m.FullName).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Email).IsRequired().HasMaxLength(200);
        builder.HasIndex(m => m.Email).IsUnique();
    }
}
