using BlogCore.Application.Interfaces;
using BlogCore.Infrastructure.Data;
using MayFlo.Specification.Builder;
using Microsoft.EntityFrameworkCore;
using MSSQLFlexCrud;
using MSSQLFlexCrud.SqlDb;

namespace BlogCore.Infrastructure.Repositories
{
    /// <summary>
    /// Extension of SqlRepository that adds Specification pattern support
    /// </summary>
    public class SpecificationSqlRepository<T> : ISpecificationRepository<T>  //SqlRepository<T>, 
        where T : class, IEntity
    {
        private readonly BlogDbContext _context;
        private readonly DbSet<T> _dbSet;

        public SpecificationSqlRepository(BlogDbContext context) //: base(context)
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

        //// Override base methods to support specifications
        //public new async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, ISpecification<T> specification = null, CancellationToken cancellationToken = default)
        //{
        //    var query = _dbSet.AsNoTracking();

        //    if (specification?.Criteria != null)
        //        query = query.Where(specification.Criteria);

        //    // Apply includes
        //    query = specification?.Includes
        //        .Aggregate(query, (current, include) => current.Include(include)) ?? query;

        //    // Apply sorting
        //    if (specification?.OrderBys.Any() == true)
        //    {
        //        var orderedQuery = query.OrderBy(specification.OrderBys[0]);
        //        for (int i = 1; i < specification.OrderBys.Count; i++)
        //        {
        //            orderedQuery = orderedQuery.ThenBy(specification.OrderBys[i]);
        //        }
        //        query = orderedQuery;
        //    }
        //    else if (specification?.OrderByDescendings.Any() == true)
        //    {
        //        var orderedQuery = query.OrderByDescending(specification.OrderByDescendings[0]);
        //        for (int i = 1; i < specification.OrderByDescendings.Count; i++)
        //        {
        //            orderedQuery = orderedQuery.ThenByDescending(specification.OrderByDescendings[i]);
        //        }
        //        query = orderedQuery;
        //    }

        //    return await query
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();
        //}

        // I commented out the above method for now because I need to research how to properly implement paging with specifications, especially when it comes to applying sorting and includes. I want to make sure I'm not missing any edge cases or optimizations before finalizing that method.
        public async Task<IReadOnlyList<T>> FindTrackedAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsTracking(); // Remove AsNoTracking()

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            return await query.ToListAsync(cancellationToken);
        }


        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbSet, spec);
        }

        //// CRUD Operations
        //public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        //{
        //    return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        //}

        //public virtual async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
        //{
        //    await _dbSet.AddAsync(entity, cancellationToken);
        //    await _context.SaveChangesAsync(cancellationToken);
        //    return entity;
        //}

        //public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        //{
        //    _dbSet.Update(entity);
        //    await _context.SaveChangesAsync(cancellationToken);
        //}

        //public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        //{
        //    var entity = await GetByIdAsync(id, cancellationToken);
        //    if (entity != null)
        //    {
        //        _dbSet.Remove(entity);
        //        await _context.SaveChangesAsync(cancellationToken);
        //    }
        //}

        //Task ISpecificationRepository<T>.CreateAsync(T entity, CancellationToken cancellationToken)
        //{
        //    return CreateAsync(entity, cancellationToken);
        //}

        //TO DO: Research into why I had to explicitly implement the CreateAsync method here to avoid a compile error about ambiguous methods. It seems like the base class and interface methods are conflicting, but I thought the 'new' keyword would resolve that. Need to investigate further.
    }
}
