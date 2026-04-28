using MayFlo.Specification.Builder;
using MSSQLFlexCrud;

namespace BlogCore.Application.Interfaces
{
    // Interface for specification repository
    public interface ISpecificationRepository<T> where T : class, IEntity
    {
        // Base CRUD
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task CreateAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        // Specification pattern methods
        Task<IReadOnlyList<T>> FindAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> FindTrackedAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, ISpecification<T> specification = null, CancellationToken cancellationToken = default);
    }
}
