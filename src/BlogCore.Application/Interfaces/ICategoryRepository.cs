using BlogCore.Core.Entities;
using MayFlo.Specification.Builder;
using MSSQLFlexCrud.Repositories;

namespace BlogCore.Application.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        // Specification pattern methods
        Task<IReadOnlyList<Category>> FindAsync(ISpecification<Category> specification, CancellationToken cancellationToken = default);
        Task<Category?> FirstOrDefaultAsync(ISpecification<Category> specification, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<Category> specification, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(ISpecification<Category> specification, CancellationToken cancellationToken = default);

        // Category-specific methods
        Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Category>> GetCategoryHierarchyAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Category>> GetSubCategoriesAsync(Guid parentCategoryId, CancellationToken cancellationToken = default);
        Task<int> GetCategoryPostCountAsync(Guid categoryId, CancellationToken cancellationToken = default);
    }
}

