using BlogCore.Application.Interfaces;
using BlogCore.Core.Entities;
using MayFlo.Specification.Builder;
using Microsoft.EntityFrameworkCore;
using MSSQLFlexCrud.DatatContext;
using MSSQLFlexCrud.SqlDb;

namespace BlogCore.Infrastructure.Repositories
{
    public class CategoryRepository : SqlRepository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Category> _dbSet;

        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Category>();
        }

        // Specification pattern implementation
        public async Task<IReadOnlyList<Category>> FindAsync(ISpecification<Category> specification, CancellationToken cancellationToken = default)
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

        public async Task<Category?> FirstOrDefaultAsync(ISpecification<Category> specification, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking();

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            query = specification.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CountAsync(ISpecification<Category> specification, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking();

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            return await query.CountAsync(cancellationToken);
        }

        public async Task<bool> AnyAsync(ISpecification<Category> specification, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking();

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            return await query.AnyAsync(cancellationToken);
        }

        // Category-specific implementations
        public async Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
        }

        public async Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.ParentCategoryId == null)
                .Include(c => c.SubCategories)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Category>> GetCategoryHierarchyAsync(CancellationToken cancellationToken = default)
        {
            var allCategories = await _dbSet
                .AsNoTracking()
                .Include(c => c.SubCategories)
                .ToListAsync(cancellationToken);

            return allCategories.Where(c => c.ParentCategoryId == null).ToList();
        }

        public async Task<IReadOnlyList<Category>> GetSubCategoriesAsync(Guid parentCategoryId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.ParentCategoryId == parentCategoryId)
                .Include(c => c.SubCategories)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetCategoryPostCountAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            var category = await _dbSet
                .AsNoTracking()
                .Include(c => c.BlogPosts)
                .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);

            return category?.BlogPosts?.Count(p => p.Status == Core.Enums.PostStatus.Published) ?? 0;
        }
    }
}
