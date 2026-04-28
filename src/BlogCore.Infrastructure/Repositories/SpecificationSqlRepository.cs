using BlogCore.Application.Interfaces;
using MayFlo.Specification.Builder;
using Microsoft.EntityFrameworkCore;
using MSSQLFlexCrud;
using MSSQLFlexCrud.DatatContext;
using MSSQLFlexCrud.SqlDb;

namespace BlogCore.Infrastructure.Repositories
{
    /// <summary>
    /// Extension of SqlRepository that adds Specification pattern support
    /// </summary>
    public class SpecificationSqlRepository<T> : SqlRepository<T>, ISpecificationRepository<T>
        where T : class, IEntity
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public SpecificationSqlRepository(AppDbContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // Specification-specific methods
        public async Task<IReadOnlyList<T>> FindAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).ToListAsync(cancellationToken);
        }

        public async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).CountAsync(cancellationToken);
        }

        public async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification).AnyAsync(cancellationToken);
        }

        // Override base methods to support specifications
        public new async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, ISpecification<T> specification = null)
        {
            var query = _dbSet.AsNoTracking();

            if (specification?.Criteria != null)
                query = query.Where(specification.Criteria);

            // Apply includes
            query = specification?.Includes
                .Aggregate(query, (current, include) => current.Include(include)) ?? query;

            // Apply sorting
            if (specification?.OrderBys.Any() == true)
            {
                var orderedQuery = query.OrderBy(specification.OrderBys[0]);
                for (int i = 1; i < specification.OrderBys.Count; i++)
                {
                    orderedQuery = orderedQuery.ThenBy(specification.OrderBys[i]);
                }
                query = orderedQuery;
            }
            else if (specification?.OrderByDescendings.Any() == true)
            {
                var orderedQuery = query.OrderByDescending(specification.OrderByDescendings[0]);
                for (int i = 1; i < specification.OrderByDescendings.Count; i++)
                {
                    orderedQuery = orderedQuery.ThenByDescending(specification.OrderByDescendings[i]);
                }
                query = orderedQuery;
            }

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbSet, spec);
        }
    }
}
