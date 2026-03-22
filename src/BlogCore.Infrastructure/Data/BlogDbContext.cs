using BlogCore.Core.Entities;
using BlogCore.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using MSSQLFlexCrud.DatatContext;

namespace BlogCore.Infrastructure.Data
{
    public class BlogDbContext : AppDbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }

        // This is where the configurations are applied
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Option 1: Apply configurations individually
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new BlogPostConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());

            // Option 2: Apply all configurations from an assembly (if you have many)
            // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
