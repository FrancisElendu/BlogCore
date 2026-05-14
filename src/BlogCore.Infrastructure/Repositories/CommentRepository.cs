using BlogCore.Application.Interfaces;
using BlogCore.Core.Entities;
using BlogCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogCore.Infrastructure.Repositories
{
    public class CommentRepository : SpecificationSqlRepository<Comment> , ICommentRepository
    {
        private readonly BlogDbContext _context;
        private readonly DbSet<Comment> _dbSet;

        public CommentRepository(BlogDbContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Comment>();
        }

        // Comment-specific implementations
        public async Task<IReadOnlyList<Comment>> GetCommentsForPostAsync(Guid postId, bool onlyApproved = true, CancellationToken cancellationToken = default)
        {
            IQueryable<Comment> query = _dbSet
                .AsNoTracking()
                .Where(c => c.BlogPostId == postId && c.ParentCommentId == null)
                .Include(c => c.Replies)
                .OrderBy(c => c.CreatedAt);

            if (onlyApproved)
            {
                query = query.Where(c => c.IsApproved);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Comment>> GetPendingCommentsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => !c.IsApproved)
                .Include(c => c.BlogPost)
                .OrderBy(c => c.CreatedAt)
                .Take(50)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Comment>> GetRecentCommentsAsync(int count = 10, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.IsApproved)
                .Include(c => c.BlogPost)
                .OrderByDescending(c => c.CreatedAt)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetCommentCountForPostAsync(Guid postId, bool onlyApproved = true, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(c => c.BlogPostId == postId);

            if (onlyApproved)
                query = query.Where(c => c.IsApproved);

            return await query.CountAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Comment>> GetCommentThreadAsync(Guid rootCommentId, CancellationToken cancellationToken = default)
        {
            var comments = await _dbSet
                .AsNoTracking()
                .Where(c => c.Id == rootCommentId || c.ParentCommentId == rootCommentId)
                .Include(c => c.Replies)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync(cancellationToken);

            return comments.Where(c => c.Id == rootCommentId).ToList();
        }

        public async Task<int> ApproveCommentAsync(Guid commentId, CancellationToken cancellationToken = default)
        {
            var comment = await _dbSet.FindAsync(new object[] { commentId }, cancellationToken);
            if (comment == null)
                return 0;

            comment.IsApproved = true;
            _dbSet.Update(comment);
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> RejectCommentAsync(Guid commentId, CancellationToken cancellationToken = default)
        {
            var comment = await _dbSet.FindAsync(new object[] { commentId }, cancellationToken);
            if (comment == null)
                return 0;

            comment.IsApproved = false;
            _dbSet.Update(comment);
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> BulkApproveCommentsAsync(IEnumerable<Guid> commentIds, CancellationToken cancellationToken = default)
        {
            var comments = await _dbSet
                .Where(c => commentIds.Contains(c.Id))
                .ToListAsync(cancellationToken);

            foreach (var comment in comments)
            {
                comment.IsApproved = true;
            }

            _dbSet.UpdateRange(comments);
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> BulkDeleteCommentsAsync(IEnumerable<Guid> commentIds, CancellationToken cancellationToken = default)
        {
            var comments = await _dbSet
                .Where(c => commentIds.Contains(c.Id))
                .ToListAsync(cancellationToken);

            _dbSet.RemoveRange(comments);
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
