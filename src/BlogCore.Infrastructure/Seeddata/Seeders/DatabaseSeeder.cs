using BlogCore.Application.Interfaces;
using BlogCore.Core.Entities;
using BlogCore.Core.Enums;
using BlogCore.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using BC = BCrypt.Net.BCrypt;

namespace BlogCore.Infrastructure.Seeddata.Seeders
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly BlogDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly IHostEnvironment _environment;
        private readonly string _seedDataPath;

        public DatabaseSeeder(
            BlogDbContext context,
            ILogger<DatabaseSeeder> logger,
            IHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
            _seedDataPath = Path.Combine(_environment.ContentRootPath, "SeedData", "Json");
            //_seedDataPath = Path.Combine(AppContext.BaseDirectory, "SeedData");
        }
        public async Task SeedAsync()
        {
            // Only seed in development environment
            if (!_environment.IsDevelopment())
            {
                _logger.LogWarning("Database seeding skipped - not in development environment");
                
                return;
            }

            try
            {
                _logger.LogInformation("Starting database seeding from JSON files...");

                //await SeedUsersAsync();
                //await SeedCategoriesAsync();
                //await SeedTagsAsync();
                //await SeedBlogPostsAsync();
                //await SeedBlogPostCategoriesAsync();
                //await SeedBlogPostTagsAsync();
                //await SeedCommentsAsync();

                await _context.SaveChangesAsync();
                _logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }


        }


        #region Individual Seed Methods

        private async Task SeedUsersAsync()
        {
            if (await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Users already exist. Skipping seeding.");
                return;
            }

            // Simple seeding - no post-processing needed
            await SeedAsync<User>("users.json", user =>
            {
                // Sync post-processing: Hash passwords
                if (user.PasswordHash == "DUMMY_HASH_WILL_BE_REPLACED")
                {
                    user.PasswordHash = BC.HashPassword("Password123!");
                }

                // Set default values if missing
                if (user.CreatedAt == default)
                    user.CreatedAt = DateTime.UtcNow;

                if (user.Id == Guid.Empty)
                    user.Id = Guid.NewGuid();

                return Task.CompletedTask;
            });           
        }


        private async Task SeedCategoriesAsync()
        {
            if (await _context.Categories.AnyAsync())
            {
                _logger.LogInformation("Categories already exist. Skipping seeding.");
                return;
            }

            await SeedAsync<Category>("categories.json", category =>
            {
                // Sync post-processing: Generate slug if missing
                if (string.IsNullOrEmpty(category.Slug))
                {
                    category.Slug = GenerateSlug(category.Name);
                }

                //// Ensure slug is unique by checking database (simplified)
                //category.Slug = EnsureUniqueSlug(category.Slug, category.Id);

                // Set default values
                if (category.CreatedAt == default)
                    category.CreatedAt = DateTime.UtcNow;

                if (category.Id == Guid.Empty)
                    category.Id = Guid.NewGuid();

                return Task.CompletedTask;
            });
        }

        private async Task SeedTagsAsync()
        {
            if (await _context.Tags.AnyAsync())
            {
                _logger.LogInformation("Tags already exist. Skipping seeding.");
                return;
            }

            await SeedAsync<Tag>("tags.json", tag =>
            {
                // Sync post-processing: Generate slug if missing
                if (string.IsNullOrEmpty(tag.Slug))
                {
                    tag.Slug = GenerateSlug(tag.Name);
                }

                // Set default values
                if (tag.CreatedAt == default)
                    tag.CreatedAt = DateTime.UtcNow;

                if (tag.Id == Guid.Empty)
                    tag.Id = Guid.NewGuid();

                return Task.CompletedTask;
            });
        }

        private async Task SeedBlogPostsAsync()
        {
            if (await _context.BlogPosts.AnyAsync())
            {
                _logger.LogInformation("Blog posts already exist. Skipping seeding.");
                return;
            }

            await SeedAsync<BlogPost>("blogposts.json", blogPost =>
            {
                // Sync post-processing: Generate excerpt from content if not provided
                if (string.IsNullOrEmpty(blogPost.Excerpt) && !string.IsNullOrEmpty(blogPost.Content))
                {
                    blogPost.Excerpt = GenerateExcerpt(blogPost.Content, 200);
                }

                // Generate slug if missing
                if (string.IsNullOrEmpty(blogPost.Slug))
                {
                    blogPost.Slug = GenerateSlug(blogPost.Title);
                }

                // Set default status based on publish date
                if (blogPost.Status == 0 && blogPost.PublishedAt <= DateTime.UtcNow)
                {
                    blogPost.Status = PostStatus.Published;
                }

                // Set default values
                if (blogPost.CreatedAt == default)
                    blogPost.CreatedAt = DateTime.UtcNow;

                if (blogPost.Id == Guid.Empty)
                    blogPost.Id = Guid.NewGuid();

                if (blogPost.ViewCount < 0)
                    blogPost.ViewCount = 0;

                if (blogPost.LikeCount < 0)
                    blogPost.LikeCount = 0;

                return Task.CompletedTask;
            });
        }

        private async Task SeedBlogPostCategoriesAsync()
        {
            if (await _context.BlogPostCategories.AnyAsync())
            {
                _logger.LogInformation("Blog post categories already exist. Skipping seeding.");
                return;
            }

            // Simple seeding - no post-processing needed
            await SeedAsync<BlogPostCategories>("blogpostcategories.json");
        }

        private async Task SeedBlogPostTagsAsync()
        {
            if (await _context.BlogPostTags.AnyAsync())
            {
                _logger.LogInformation("Blog post tags already exist. Skipping seeding.");
                return;
            }

            // Simple seeding - no post-processing needed
            await SeedAsync<BlogPostTags>("blogposttags.json");
        }

        private async Task SeedCommentsAsync()
        {
            if (await _context.Comments.AnyAsync())
            {
                _logger.LogInformation("Comments already exist. Skipping seeding.");
                return;
            }

            await SeedAsync<Comment>("comments.json", comment =>
            {
                // Sync post-processing: Trim content if too long
                if (comment.Content?.Length > 500)
                    comment.Content = comment.Content.Substring(0, 500);

                // Set default values
                if (comment.CreatedAt == default)
                    comment.CreatedAt = DateTime.UtcNow;

                if (comment.Id == Guid.Empty)
                    comment.Id = Guid.NewGuid();

                return Task.CompletedTask;
            });
        }

        //var jsonPath = Path.Combine(_seedDataPath, "users.json");

        //// Use extension method
        //if (!await jsonPath.ValidateJsonFileAsync(_logger))
        //{
        //    return;
        //}


        //var jsonContent = await File.ReadAllTextAsync(jsonPath);

        //// Deserialize directly into List<User>
        //var users = JsonSerializer.Deserialize<List<User>>(jsonContent);

        //if (users == null) return;

        //// Hash passwords after deserialization
        //foreach (var user in users)
        //{
        //    if (user.PasswordHash == "DUMMY_HASH_WILL_BE_REPLACED")
        //    {
        //        user.PasswordHash = BC.HashPassword("Password123!");
        //    }
        //}

        //await _context.Users.AddRangeAsync(users);
        //_logger.LogInformation("Seeded {Count} users from JSON.", users.Count);


        #endregion


        #region Generic Seeding Method

        private async Task SeedAsync<T>(string fileName, Func<T, Task>? postProcess = null) where T : class
        {
            var jsonPath = Path.Combine(_seedDataPath, fileName);

            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("Seed file not found: {FilePath}", jsonPath);
                return;
            }

            if (!await jsonPath.ValidateJsonFileAsync(_logger))
            {
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var items = JsonSerializer.Deserialize<List<T>>(jsonContent, options);

            if (items == null || !items.Any())
            {
                _logger.LogWarning("No items found in {FileName} or deserialization failed.", fileName);
                return;
            }

            // Apply post-processing if provided
            if (postProcess != null)
            {
                foreach (var item in items)
                {
                    await postProcess(item);
                }
            }

            // Add to database
            await _context.Set<T>().AddRangeAsync(items);
            _logger.LogInformation("Seeded {Count} {Entity} from {FileName}.",
                items.Count, typeof(T).Name, fileName);
        }

        #endregion

        #region Helper Methods

        private string GenerateSlug(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            var slug = name.ToLower().Trim()
                .Replace(" ", "-")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("?", "")
                .Replace("!", "")
                .Replace(":", "")
                .Replace(";", "")
                .Replace("/", "-")
                .Replace("\\", "-")
                .Replace("&", "and")
                .Replace("#", "sharp")
                .Replace("+", "plus")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace("{", "")
                .Replace("}", "");

            // Remove multiple consecutive dashes
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");

            // Limit length
            if (slug.Length > 200)
                slug = slug.Substring(0, 200);

            return slug.Trim('-');
        }

        private string GenerateExcerpt(string content, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            // Remove HTML tags if any
            var plainText = System.Text.RegularExpressions.Regex.Replace(content, "<.*?>", string.Empty);

            if (plainText.Length <= maxLength)
                return plainText;

            // Find the last space within the max length
            var excerpt = plainText.Substring(0, maxLength);
            var lastSpace = excerpt.LastIndexOf(' ');

            if (lastSpace > 0)
                excerpt = excerpt.Substring(0, lastSpace);

            return excerpt + "...";
        }


        private string EnsureUniqueSlug(string slug, Guid currentId)
        {
            // This is a simplified version - in reality you'd check the database
            // For seeding purposes with predefined GUIDs, this is usually fine
            return slug;
        }

        #endregion

        //private async Task SeedDataAsync<T>(string fileName) where T : class
        //{
        //    if (await _context.Set<T>().AnyAsync())
        //    {
        //        _logger.LogInformation("{EntityName} already exist. Skipping seeding.", typeof(T).Name);
        //        return;
        //    }
        //    var jsonPath = Path.Combine(_seedDataPath, fileName);
        //    if (!await jsonPath.ValidateJsonFileAsync(_logger))
        //    {
        //        return;
        //    }
        //    var jsonContent = await File.ReadAllTextAsync(jsonPath);
        //    var entities = JsonSerializer.Deserialize<List<T>>(jsonContent);
        //    if (entities == null) return;
        //    await _context.Set<T>().AddRangeAsync(entities);
        //    _logger.LogInformation("Seeded {Count} {EntityName} from JSON.", entities.Count, typeof(T).Name);
        //}



    }
}