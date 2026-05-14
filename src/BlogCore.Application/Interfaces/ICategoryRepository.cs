using BlogCore.Core.Entities;
using MSSQLFlexCrud.Repositories;

namespace BlogCore.Application.Interfaces
{
    public interface ICategoryRepository : ISpecificationRepository<Category>//, IRepository<Category>
    {
        //// Category-specific methods
        Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Category>> GetCategoryHierarchyAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Category>> GetSubCategoriesAsync(Guid parentCategoryId, CancellationToken cancellationToken = default);
        Task<int> GetCategoryPostCountAsync(Guid categoryId, CancellationToken cancellationToken = default);
    }
}

