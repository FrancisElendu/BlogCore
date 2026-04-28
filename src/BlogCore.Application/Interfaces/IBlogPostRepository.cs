using BlogCore.Core.Entities;
using MSSQLFlexCrud.Repositories;

namespace BlogCore.Application.Interfaces
{
    public interface IBlogPostRepository : ISpecificationRepository<BlogPost>, IRepository<BlogPost>
    {
        // Domain-specific methods
        Task<IReadOnlyList<BlogPost>> GetPopularPostsAsync(int count, CancellationToken cancellationToken = default);
        Task<Dictionary<string, int>> GetPostsCountByStatusAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<BlogPost>> GetPostsByAuthorAsync(Guid authorId, int page, int pageSize, CancellationToken cancellationToken = default);

        // Optional: If you need total count
        Task<(IReadOnlyList<BlogPost> Posts, int TotalCount)> GetPostsByAuthorWithCountAsync(Guid authorId, int page, int pageSize, CancellationToken cancellationToken = default);

        // Optional: If you need sorting
        Task<IReadOnlyList<BlogPost>> GetPostsByAuthorAsync(Guid authorId, int page, int pageSize, string sortBy="createdAt", bool descending=false, CancellationToken cancellationToken = default);

    }
}

