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

            // Many-to-many with Categories (using explicit junction entity)
            builder.HasMany(b => b.Categories)
                   .WithMany(c => c.BlogPosts)
                   .UsingEntity<BlogPostCategories>(
                       j => j.HasOne(bpc => bpc.Category)
                             .WithMany()
                             .HasForeignKey(bpc => bpc.CategoryId),
                       j => j.HasOne(bpc => bpc.BlogPost)
                             .WithMany()
                             .HasForeignKey(bpc => bpc.BlogPostId),
                       j =>
                       {
                           j.HasKey(bpc => new { bpc.BlogPostId, bpc.CategoryId });
                           j.HasIndex(bpc => bpc.CategoryId);  // Add index for CategoryId
                           j.HasIndex(bpc => bpc.BlogPostId); // Add index for BlogPostId
                       }
                   );

            // ADD THIS: Many-to-many with Tags (using explicit junction entity)
            builder.HasMany(b => b.Tags)
                   .WithMany(t => t.BlogPosts)
                   .UsingEntity<BlogPostTags>(
                       j => j.HasOne(bpt => bpt.Tag)
                             .WithMany()
                             .HasForeignKey(bpt => bpt.TagId),
                       j => j.HasOne(bpt => bpt.BlogPost)
                             .WithMany()
                             .HasForeignKey(bpt => bpt.BlogPostId),
                       j =>
                       {
                           j.HasKey(bpt => new { bpt.BlogPostId, bpt.TagId });
                           j.HasIndex(bpt => bpt.TagId);      // Add index for TagId
                           j.HasIndex(bpt => bpt.BlogPostId); // Add index for BlogPostId
                       }
                   );
        }
    }
}
