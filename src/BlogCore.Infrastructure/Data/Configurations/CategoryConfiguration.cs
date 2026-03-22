using BlogCore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogCore.Infrastructure.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasIndex(c => c.Slug).IsUnique();
            builder.HasMany(c => c.SubCategories)
                   .WithOne(c => c.ParentCategory)
                   .HasForeignKey(c => c.ParentCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.Name)
               .IsRequired()
               .HasMaxLength(100);
        }
    }
}
