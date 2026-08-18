using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(c => c.Name).IsRequired().HasMaxLength(150);

        // Self-referencing hierarchy. MUST be Restrict - SQL Server refuses to
        // create a Cascade FK here because deleting a parent could cascade
        // into itself through more than one path ("may cause cycles or
        // multiple cascade paths"). Deleting a parent category is a deliberate,
        // explicit action in the app instead.
        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
