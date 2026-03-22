using BlogCore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogCore.Infrastructure.Data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Content)
                   .IsRequired()
                   .HasMaxLength(500);

            // Configure the self-referential relationship with NO ACTION
            builder.HasOne(c => c.ParentComment)
                   .WithMany(c => c.Replies)
                   .HasForeignKey(c => c.ParentCommentId)
                   .OnDelete(DeleteBehavior.NoAction);  // Key change: No Action instead of Cascade

            // BlogPost relationship remains Cascade
            builder.HasOne(c => c.BlogPost)
                   .WithMany(b => b.Comments)
                   .HasForeignKey(c => c.BlogPostId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Add index for better performance
            builder.HasIndex(c => c.BlogPostId);
            builder.HasIndex(c => c.ParentCommentId);
            builder.HasIndex(c => c.CreatedAt);
        }
    }
}
