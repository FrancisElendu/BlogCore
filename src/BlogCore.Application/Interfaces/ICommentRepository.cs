using BlogCore.Core.Entities;
using MSSQLFlexCrud.Repositories;

namespace BlogCore.Application.Interfaces
{
    public interface ICommentRepository : ISpecificationRepository<Comment>//, IRepository<Comment>
    {
        //// Comment-specific methods
        Task<IReadOnlyList<Comment>> GetCommentsForPostAsync(Guid postId, bool onlyApproved = true, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Comment>> GetPendingCommentsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Comment>> GetRecentCommentsAsync(int count = 10, CancellationToken cancellationToken = default);
        Task<int> GetCommentCountForPostAsync(Guid postId, bool onlyApproved = true, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Comment>> GetCommentThreadAsync(Guid rootCommentId, CancellationToken cancellationToken = default);
        Task<int> ApproveCommentAsync(Guid commentId, CancellationToken cancellationToken = default);
        Task<int> RejectCommentAsync(Guid commentId, CancellationToken cancellationToken = default);
        Task<int> BulkApproveCommentsAsync(IEnumerable<Guid> commentIds, CancellationToken cancellationToken = default);
        Task<int> BulkDeleteCommentsAsync(IEnumerable<Guid> commentIds, CancellationToken cancellationToken = default);
    }
}
