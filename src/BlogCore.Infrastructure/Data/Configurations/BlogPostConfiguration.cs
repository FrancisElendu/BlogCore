using BlogCore.Core.Entities;
using BlogCore.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogCore.Infrastructure.Data.Configurations
{
    public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
    {
        public void Configure(EntityTypeBuilder<BlogPost> builder)
        {
            builder.HasIndex(b => b.Slug).IsUnique();
            builder.HasIndex(b => b.PublishedAt);
            builder.Property(b => b.Status)
                   .HasDefaultValue(PostStatus.Draft);

            // For many-to-many relationship with explicit junction table
            builder.HasMany(b => b.Categories)
                   .WithMany(c => c.BlogPosts)
                   .UsingEntity<BlogPostCategory>(
                       j => j.HasOne(bpc => bpc.Category)
                             .WithMany()
                             .HasForeignKey(bpc => bpc.CategoryId),
                       j => j.HasOne(bpc => bpc.BlogPost)
                             .WithMany()
                             .HasForeignKey(bpc => bpc.BlogPostId),
                       j => j.HasKey(bpc => new { bpc.BlogPostId, bpc.CategoryId })
                   );
        }
    }
}
