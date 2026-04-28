using BlogCore.Core.Entities;
using MayFlo.Specification.Builder;
using MSSQLFlexCrud.Repositories;

namespace BlogCore.Application.Interfaces
{
    public interface ITagRepository : IRepository<Tag>
    {
        // Specification pattern methods
        Task<IReadOnlyList<Tag>> FindAsync(ISpecification<Tag> specification, CancellationToken cancellationToken = default);
        Task<Tag?> FirstOrDefaultAsync(ISpecification<Tag> specification, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<Tag> specification, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(ISpecification<Tag> specification, CancellationToken cancellationToken = default);

        // Tag-specific methods
        Task<Tag?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Tag>> GetPopularTagsAsync(int count = 20, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Tag>> GetTagsForPostAsync(Guid postId, CancellationToken cancellationToken = default);
        Task<int> GetTagPostCountAsync(Guid tagId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Tag>> SearchTagsAsync(string searchTerm, int limit = 10, CancellationToken cancellationToken = default);
        Task<Tag> CreateOrGetTagAsync(string name, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Tag>> CreateOrGetTagsAsync(IEnumerable<string> tagNames, CancellationToken cancellationToken = default);
        Task<bool> IsTagInUseAsync(Guid tagId, CancellationToken cancellationToken = default);
    }
}
