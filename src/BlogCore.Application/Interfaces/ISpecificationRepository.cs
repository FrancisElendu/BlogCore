using MayFlo.Specification.Builder;
using MSSQLFlexCrud;

namespace BlogCore.Application.Interfaces
{
    // Interface for specification repository
    public interface ISpecificationRepository<T> where T : class, IEntity
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T>> FindAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, ISpecification<T> specification = null);
    }
}
