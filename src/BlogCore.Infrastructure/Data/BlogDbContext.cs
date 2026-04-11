using BlogCore.Core.Entities;
using BlogCore.Infrastructure.Constants;
using BlogCore.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MSSQLFlexCrud.DatatContext;

namespace BlogCore.Infrastructure.Data
{
    public class BlogDbContext : AppDbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }

        // Identity DbSets
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<IdentityRole<Guid>> Roles { get; set; }
        public DbSet<IdentityUserRole<Guid>> UserRoles { get; set; }
        public DbSet<IdentityUserClaim<Guid>> UserClaims { get; set; }
        public DbSet<IdentityUserLogin<Guid>> UserLogins { get; set; }
        public DbSet<IdentityUserToken<Guid>> UserTokens { get; set; }
        public DbSet<IdentityRoleClaim<Guid>> RoleClaims { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        //public DbSet<User> Users { get; set; }
        public DbSet<BlogPostCategories> BlogPostCategories { get; set; }
        public DbSet<BlogPostTags> BlogPostTags { get; set; }

        // This is where the configurations are applied
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Configure Identity tables manually
            ConfigureIdentityTables(modelBuilder);

            // Option 1: Apply configurations individually
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new BlogPostConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());

            // Option 2: Apply all configurations from an assembly (if you have many)
            // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Call base
            base.OnModelCreating(modelBuilder);
        }

        //pppp
        private void ConfigureIdentityTables(ModelBuilder builder)
        {
            // Configure ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users", SchemaNames.Identity);
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DisplayName).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasMany(e => e.BlogPosts)
                    .WithOne(b => b.Author)
                    .HasForeignKey(b => b.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure IdentityRole
            builder.Entity<IdentityRole<Guid>>(entity =>
            {
                entity.ToTable("Roles", SchemaNames.Identity);
                entity.HasKey(e => e.Id);
            });

            // Configure IdentityUserRole
            builder.Entity<IdentityUserRole<Guid>>(entity =>
            {
                entity.ToTable("UserRoles", SchemaNames.Identity);
                entity.HasKey(e => new { e.UserId, e.RoleId });
            });

            // Configure IdentityUserClaim
            builder.Entity<IdentityUserClaim<Guid>>(entity =>
            {
                entity.ToTable("UserClaims", SchemaNames.Identity);
                entity.HasKey(e => e.Id);
            });

            // Configure IdentityUserLogin
            builder.Entity<IdentityUserLogin<Guid>>(entity =>
            {
                entity.ToTable("UserLogins", SchemaNames.Identity);
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
            });

            // Configure IdentityUserToken
            builder.Entity<IdentityUserToken<Guid>>(entity =>
            {
                entity.ToTable("UserTokens", SchemaNames.Identity);
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
            });

            // Configure IdentityRoleClaim
            builder.Entity<IdentityRoleClaim<Guid>>(entity =>
            {
                entity.ToTable("RoleClaims", SchemaNames.Identity);
                entity.HasKey(e => e.Id);
            });
        }

        //llll
    }
}
