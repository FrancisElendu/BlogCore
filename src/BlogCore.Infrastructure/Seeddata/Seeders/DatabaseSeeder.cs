using BlogCore.Application.Interfaces;
using BlogCore.Core.Entities;
using BlogCore.Core.Enums;
using BlogCore.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly string _seedDataPath;

        public DatabaseSeeder(
            BlogDbContext context,
            ILogger<DatabaseSeeder> logger,
            IHostEnvironment environment, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
            _userManager = userManager;
            _roleManager = roleManager;
            //_seedDataPath = Path.Combine(_environment.ContentRootPath, "SeedData", "Json");
            //_seedDataPath = Path.Combine(AppContext.BaseDirectory, "SeedData");
            // _seedDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SeedData", "Json");
            _seedDataPath = Path.Combine(_environment.ContentRootPath, "..", "BlogCore.Infrastructure", "SeedData", "Json");
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

                // Seed Identity tables (in correct order)
                await SeedRolesAsync();
                await SeedUsersAsync();
                await SeedUserRolesAsync();
                await SeedRoleClaimsAsync();
                await SeedUserClaimsAsync();
                await SeedUserLoginsAsync();
                await SeedUserTokensAsync();


                // Seed Blog tables
                await SeedCategoriesAsync();
                await SeedTagsAsync();
                await SeedBlogPostsAsync();
                await SeedBlogPostCategoriesAsync();
                await SeedBlogPostTagsAsync();
                await SeedCommentsAsync();

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


        #region Identity Tables Seeding

        private async Task SeedRolesAsync()
        {
            if (await _roleManager.Roles.AnyAsync())
            {
                _logger.LogInformation("Roles already exist. Skipping seeding.");
                return;
            }

            var jsonPath = Path.Combine(_seedDataPath, "roles.json");
            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("Roles seed file not found: {JsonPath}", jsonPath);
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Deserialize directly into IdentityRole<Guid>
            var roles = JsonSerializer.Deserialize<List<IdentityRole<Guid>>>(jsonContent, options);

            if (roles == null || !roles.Any())
            {
                _logger.LogWarning("No roles found in roles.json");
                return;
            }

            foreach (var role in roles)
            {
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Created role: {RoleName}", role.Name);
                }
                else
                {
                    _logger.LogError("Failed to create role {RoleName}: {Errors}",
                        role.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
        private async Task SeedUsersAsync()
        {
            if (await _userManager.Users.AnyAsync())
            {
                _logger.LogInformation("Users already exist. Skipping seeding.");
                return;
            }

            var jsonPath = Path.Combine(_seedDataPath, "users.json");
            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("Users seed file not found: {JsonPath}", jsonPath);
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Deserialize directly into ApplicationUser
            var users = JsonSerializer.Deserialize<List<ApplicationUser>>(jsonContent, options);

            if (users == null || !users.Any())
            {
                _logger.LogWarning("No users found in users.json");
                return;
            }

            foreach (var user in users)
            {
                // Create user with default password
                var result = await _userManager.CreateAsync(user, "Password123!");

                if (result.Succeeded)
                {
                    _logger.LogInformation("Created user: {UserName}", user.UserName);
                }
                else
                {
                    _logger.LogError("Failed to create user {UserName}: {Errors}",
                        user.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
        private async Task SeedUserRolesAsync()
        {
            if (await _context.UserRoles.AnyAsync())
            {
                _logger.LogInformation("User roles already exist. Skipping seeding.");
                return;
            }

            await SeedAsync<IdentityUserRole<Guid>>("userroles.json");
        }
        private async Task SeedRoleClaimsAsync()
        {
            if (await _context.RoleClaims.AnyAsync())
            {
                _logger.LogInformation("Role claims already exist. Skipping seeding.");
                return;
            }

            await SeedAsync<IdentityRoleClaim<Guid>>("roleclaims.json");
        }
        private async Task SeedUserClaimsAsync()
        {
            if (await _context.UserClaims.AnyAsync())
            {
                _logger.LogInformation("User claims already exist. Skipping seeding.");
                return;
            }

            await SeedAsync<IdentityUserClaim<Guid>>("userclaims.json");
        }
        private async Task SeedUserLoginsAsync()
        {
            if (await _context.UserLogins.AnyAsync())
            {
                _logger.LogInformation("User logins already exist. Skipping seeding.");
                return;
            }

            await SeedAsync<IdentityUserLogin<Guid>>("userlogins.json");
        }
        private async Task SeedUserTokensAsync()
        {
            if (await _context.UserTokens.AnyAsync())
            {
                _logger.LogInformation("User tokens already exist. Skipping seeding.");
                return;
            }

            await SeedAsync<IdentityUserToken<Guid>>("usertokens.json");
        }
        #endregion

        #region Individual Seed Methods

        //private async Task SeedUsersAsync()
        //{
        //    if (await _context.Users.AnyAsync())
        //    {
        //        _logger.LogInformation("Users already exist. Skipping seeding.");
        //        return;
        //    }

        //    // Simple seeding - no post-processing needed
        //    await SeedAsync<User>("users.json", user =>
        //    {
        //        // Sync post-processing: Hash passwords
        //        if (user.PasswordHash == "DUMMY_HASH_WILL_BE_REPLACED")
        //        {
        //            user.PasswordHash = BC.HashPassword("Password123!");
        //        }

        //        // Set default values if missing
        //        if (user.CreatedAt == default)
        //            user.CreatedAt = DateTime.UtcNow;

        //        if (user.Id == Guid.Empty)
        //            user.Id = Guid.NewGuid();

        //        return Task.CompletedTask;
        //    });           
        //}


        private async Task SeedCategoriesAsync()
        {
            if (await _context.Categories.AnyAsync())
            {
                _logger.LogInformation("Categories already exist. Skipping seeding.");
                return;
            }

            //await SeedAsync<Category>("categories.json", category =>   //if this is used then return Task.CompletedTask; 
            await SeedAsync<Category>("categories.json", async category =>
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

                
                await Task.CompletedTask;
                //return Task.CompletedTask;
            });
        }

        private async Task SeedTagsAsync()
        {
            if (await _context.Tags.AnyAsync())
            {
                _logger.LogInformation("Tags already exist. Skipping seeding.");
                return;
            }

            //await SeedAsync<Tag>("tags.json", tag =>  //if this is used then return Task.CompletedTask;
            await SeedAsync<Tag>("tags.json", async tag =>
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

                await Task.CompletedTask;

                //return Task.CompletedTask;
            });
        }

        private async Task SeedBlogPostsAsync()
        {
            if (await _context.BlogPosts.AnyAsync())
            {
                _logger.LogInformation("Blog posts already exist. Skipping seeding.");
                return;
            }

            //await SeedAsync<BlogPost>("blogposts.json", blogPost => //if this is used then return Task.CompletedTask;
            await SeedAsync<BlogPost>("blogposts.json", async blogPost =>
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

                await Task.CompletedTask;

                //return Task.CompletedTask;
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

            //await SeedAsync<Comment>("comments.json", comment =>  //if this is used then return Task.CompletedTask;
            await SeedAsync<Comment>("comments.json", async comment =>    
            {
                // Sync post-processing: Trim content if too long
                if (comment.Content?.Length > 500)
                    comment.Content = comment.Content.Substring(0, 500);

                // Set default values
                if (comment.CreatedAt == default)
                    comment.CreatedAt = DateTime.UtcNow;

                if (comment.Id == Guid.Empty)
                    comment.Id = Guid.NewGuid();

                await Task.CompletedTask;

                //return Task.CompletedTask;
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