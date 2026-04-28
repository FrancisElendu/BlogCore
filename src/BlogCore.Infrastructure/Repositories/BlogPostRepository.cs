using BlogCore.Application.Interfaces;
using BlogCore.Core.Entities;
using MayFlo.Specification.Builder;
using Microsoft.EntityFrameworkCore;
using MSSQLFlexCrud.DatatContext;
using MSSQLFlexCrud.SqlDb;

namespace BlogCore.Infrastructure.Repositories
{
    public class BlogPostRepository : SqlRepository<BlogPost>, IBlogPostRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<BlogPost> _dbSet;

        public BlogPostRepository(AppDbContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<BlogPost>();
        }

        // Specification pattern implementation
        public async Task<IReadOnlyList<BlogPost>> FindAsync(ISpecification<BlogPost> specification, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking();

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            query = specification.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            // Apply sorting
            if (specification.OrderBys.Any())
            {
                var orderedQuery = query.OrderBy(specification.OrderBys[0]);
                for (int i = 1; i < specification.OrderBys.Count; i++)
                {
                    orderedQuery = orderedQuery.ThenBy(specification.OrderBys[i]);
                }
                query = orderedQuery;
            }
            else if (specification.OrderByDescendings.Any())
            {
                var orderedQuery = query.OrderByDescending(specification.OrderByDescendings[0]);
                for (int i = 1; i < specification.OrderByDescendings.Count; i++)
                {
                    orderedQuery = orderedQuery.ThenByDescending(specification.OrderByDescendings[i]);
                }
                query = orderedQuery;
            }

            if (specification.IsPagingEnabled)
                query = query.Skip(specification.Skip).Take(specification.Take);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<BlogPost?> FirstOrDefaultAsync(ISpecification<BlogPost> specification, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking();

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            query = specification.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CountAsync(ISpecification<BlogPost> specification, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking();

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            return await query.CountAsync(cancellationToken);
        }

        public async Task<bool> AnyAsync(ISpecification<BlogPost> specification, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking();

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            return await query.AnyAsync(cancellationToken);
        }
    }
}
