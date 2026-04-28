using BlogCore.Application.Interfaces;
using BlogCore.Core.Entities;
using BlogCore.Infrastructure.Data;
using MayFlo.Specification.Builder;
using Microsoft.EntityFrameworkCore;

namespace BlogCore.Infrastructure.Repositories
{
    public class TagRepository : SpecificationSqlRepository<Tag>, ITagRepository
    {
        private readonly BlogDbContext _context;
        private readonly DbSet<Tag> _dbSet;

        public TagRepository(BlogDbContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Tag>();
        }

        // Tag-specific implementations
        public async Task<Tag?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(t => t.BlogPosts)
                .FirstOrDefaultAsync(t => t.Slug == slug, cancellationToken);
        }

        public async Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower(), cancellationToken);
        }

        public async Task<IReadOnlyList<Tag>> GetPopularTagsAsync(int count = 20, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(t => t.BlogPosts)
                .OrderByDescending(t => t.BlogPosts.Count(p => p.Status == Core.Enums.PostStatus.Published))
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Tag>> GetTagsForPostAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            var blogPost = await _context.BlogPosts
                .AsNoTracking()
                .Include(bp => bp.Tags)
                .FirstOrDefaultAsync(bp => bp.Id == postId, cancellationToken);

            return blogPost?.Tags.ToList() ?? new List<Tag>();
        }

        public async Task<int> GetTagPostCountAsync(Guid tagId, CancellationToken cancellationToken = default)
        {
            var tag = await _dbSet
                .AsNoTracking()
                .Include(t => t.BlogPosts)
                .FirstOrDefaultAsync(t => t.Id == tagId, cancellationToken);

            return tag?.BlogPosts.Count(p => p.Status == Core.Enums.PostStatus.Published) ?? 0;
        }

        public async Task<IReadOnlyList<Tag>> SearchTagsAsync(string searchTerm, int limit = 10, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Tag>();

            return await _dbSet
                .AsNoTracking()
                .Where(t => t.Name.Contains(searchTerm) || t.Slug.Contains(searchTerm))
                .OrderBy(t => t.Name)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }

        public async Task<Tag> CreateOrGetTagAsync(string name, CancellationToken cancellationToken = default)
        {
            var existingTag = await GetByNameAsync(name, cancellationToken);
            if (existingTag != null)
                return existingTag;

            var slug = GenerateSlug(name);

            // Check if slug exists and make it unique if needed
            var slugExists = await AnyAsync(
                new SpecificationBuilder<Tag>().Where(t => t.Slug == slug).Build(),
                cancellationToken);

            if (slugExists)
            {
                slug = $"{slug}-{Guid.NewGuid().ToString().Substring(0, 8)}";
            }

            var tag = new Tag
            {
                Name = name,
                Slug = slug,
                CreatedAt = DateTime.UtcNow
            };

            await CreateAsync(tag);
            return tag;
        }

        public async Task<IReadOnlyList<Tag>> CreateOrGetTagsAsync(IEnumerable<string> tagNames, CancellationToken cancellationToken = default)
        {
            var tags = new List<Tag>();

            foreach (var name in tagNames.Distinct())
            {
                var tag = await CreateOrGetTagAsync(name, cancellationToken);
                tags.Add(tag);
            }

            return tags;
        }

        public async Task<bool> IsTagInUseAsync(Guid tagId, CancellationToken cancellationToken = default)
        {
            return await _context.BlogPosts
                .AnyAsync(bp => bp.Tags.Any(t => t.Id == tagId), cancellationToken);
        }

        private string GenerateSlug(string name)
        {
            var slug = name.ToLower().Trim();
            slug = slug.Replace(" ", "-");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\-+", "-");
            return slug.Trim('-');
        }
    }
}
