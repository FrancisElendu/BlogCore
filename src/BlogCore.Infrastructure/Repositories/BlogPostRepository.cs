using BlogCore.Application.Interfaces;
using BlogCore.Core.Entities;
using BlogCore.Infrastructure.Data;
using MayFlo.Specification.Builder;
using Microsoft.EntityFrameworkCore;

namespace BlogCore.Infrastructure.Repositories
{
    public class BlogPostRepository : SpecificationSqlRepository<BlogPost>, IBlogPostRepository
    {
        private readonly BlogDbContext _context;
        private readonly DbSet<BlogPost> _dbSet;

        public BlogPostRepository(BlogDbContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<BlogPost>();
        }

        public async Task<IReadOnlyList<BlogPost>> GetPopularPostsAsync(int count, CancellationToken cancellationToken = default)
        {
            var spec = new SpecificationBuilder<BlogPost>()
                .Where(p => p.Status == Core.Enums.PostStatus.Published)
                .OrderByDescending(p => p.ViewCount)
                .Take(count)
                .Include(p => p.Categories)
                .Include(p => p.Tags)
                .Build();

            return await FindAsync(spec, cancellationToken);
        }

        public async Task<Dictionary<string, int>> GetPostsCountByStatusAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .GroupBy(p => p.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToDictionaryAsync(k => k.Status, v => v.Count, cancellationToken);
        }
        public async Task<IReadOnlyList<BlogPost>> GetPostsByAuthorAsync(Guid authorId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var spec = new SpecificationBuilder<BlogPost>()
        .Where(p => p.AuthorId == authorId && p.Status == Core.Enums.PostStatus.Published)
        .OrderByDescending(p => p.PublishedAt)
        .Page(page, pageSize)
        .Include(p => p.Categories)
        .Include(p => p.Tags)
        .Build();

            return await FindAsync(spec, cancellationToken);
        }

        public async Task<IReadOnlyList<BlogPost>> GetPostsByAuthorAsync(Guid authorId, int page, int pageSize, string sortBy="createdAt", bool descending=true, CancellationToken cancellationToken = default)
        {
            var builder = new SpecificationBuilder<BlogPost>()
        .Where(p => p.AuthorId == authorId);

            // Apply dynamic sorting
            switch (sortBy.ToLower())
            {
                case "title":
                    if (descending)
                        builder.OrderByDescending(p => p.Title);
                    else
                        builder.OrderBy(p => p.Title);
                    break;
                case "viewcount":
                    if (descending)
                        builder.OrderByDescending(p => p.ViewCount);
                    else
                        builder.OrderBy(p => p.ViewCount);
                    break;
                case "publishedat":
                    if (descending)
                        builder.OrderByDescending(p => p.PublishedAt);
                    else
                        builder.OrderBy(p => p.PublishedAt);
                    break;
                default: // createdAt
                    if (descending)
                        builder.OrderByDescending(p => p.CreatedAt);
                    else
                        builder.OrderBy(p => p.CreatedAt);
                    break;
            }

            var spec = builder
                .Page(page, pageSize)
                .Include(p => p.Categories)
                .Include(p => p.Tags)
                .Include(p => p.Author)
                .Build();

            return await FindAsync(spec, cancellationToken);
            ////var spec = new SpecificationBuilder<BlogPost>()
            ////.Where(p => p.AuthorId == authorId)
            ////.OrderByDescending(p => p.CreatedAt)
            ////.Page(page, pageSize)
            ////.Include(p => p.Categories)
            ////.Include(p => p.Tags)
            ////.Include(p => p.Author) // Include author details if needed
            ////.Build();

            ////return await FindAsync(spec, cancellationToken);
        }

        public async Task<(IReadOnlyList<BlogPost> Posts, int TotalCount)> GetPostsByAuthorWithCountAsync(Guid authorId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var builder = new SpecificationBuilder<BlogPost>()
        .Where(p => p.AuthorId == authorId);

            // Get total count
            var countSpec = builder.Build();
            var totalCount = await CountAsync(countSpec, cancellationToken);

            // Get paged results
            var dataSpec = builder
                .OrderByDescending(p => p.CreatedAt)
                .Page(page, pageSize)
                .Include(p => p.Categories)
                .Include(p => p.Tags)
                .Build();

            var posts = await FindAsync(dataSpec, cancellationToken);

            return (posts, totalCount);
        }

        


        // Specification pattern implementation
        //public async Task<IReadOnlyList<BlogPost>> FindAsync(ISpecification<BlogPost> specification, CancellationToken cancellationToken = default)
        //{
        //    var query = _dbSet.AsNoTracking();

        //    if (specification.Criteria != null)
        //        query = query.Where(specification.Criteria);

        //    query = specification.Includes
        //        .Aggregate(query, (current, include) => current.Include(include));

        //    // Apply sorting
        //    if (specification.OrderBys.Any())
        //    {
        //        var orderedQuery = query.OrderBy(specification.OrderBys[0]);
        //        for (int i = 1; i < specification.OrderBys.Count; i++)
        //        {
        //            orderedQuery = orderedQuery.ThenBy(specification.OrderBys[i]);
        //        }
        //        query = orderedQuery;
        //    }
        //    else if (specification.OrderByDescendings.Any())
        //    {
        //        var orderedQuery = query.OrderByDescending(specification.OrderByDescendings[0]);
        //        for (int i = 1; i < specification.OrderByDescendings.Count; i++)
        //        {
        //            orderedQuery = orderedQuery.ThenByDescending(specification.OrderByDescendings[i]);
        //        }
        //        query = orderedQuery;
        //    }

        //    if (specification.IsPagingEnabled)
        //        query = query.Skip(specification.Skip).Take(specification.Take);

        //    return await query.ToListAsync(cancellationToken);
        //}

        //public async Task<BlogPost?> FirstOrDefaultAsync(ISpecification<BlogPost> specification, CancellationToken cancellationToken = default)
        //{
        //    var query = _dbSet.AsNoTracking();

        //    if (specification.Criteria != null)
        //        query = query.Where(specification.Criteria);

        //    query = specification.Includes
        //        .Aggregate(query, (current, include) => current.Include(include));

        //    return await query.FirstOrDefaultAsync(cancellationToken);
        //}

        //public async Task<int> CountAsync(ISpecification<BlogPost> specification, CancellationToken cancellationToken = default)
        //{
        //    var query = _dbSet.AsNoTracking();

        //    if (specification.Criteria != null)
        //        query = query.Where(specification.Criteria);

        //    return await query.CountAsync(cancellationToken);
        //}

        //public async Task<bool> AnyAsync(ISpecification<BlogPost> specification, CancellationToken cancellationToken = default)
        //{
        //    var query = _dbSet.AsNoTracking();

        //    if (specification.Criteria != null)
        //        query = query.Where(specification.Criteria);

        //    return await query.AnyAsync(cancellationToken);
        //}


        // Add any blog post-specific methods here

    }
}
