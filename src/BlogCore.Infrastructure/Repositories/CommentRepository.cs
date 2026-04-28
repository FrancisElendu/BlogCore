using BlogCore.Application.Interfaces;
using BlogCore.Core.Entities;
using BlogCore.Infrastructure.Data;
using MayFlo.Specification.Builder;
using Microsoft.EntityFrameworkCore;
using MSSQLFlexCrud.DatatContext;
using MSSQLFlexCrud.SqlDb;

namespace BlogCore.Infrastructure.Repositories
{
    public class CommentRepository : SqlRepository<Comment>, ICommentRepository
    {
        private readonly BlogDbContext _context;
        private readonly DbSet<Comment> _dbSet;

        public CommentRepository(BlogDbContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Comment>();
        }

        // Specification pattern implementation
        public async Task<IReadOnlyList<Comment>> FindAsync(ISpecification<Comment> specification, CancellationToken cancellationToken = default)
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

        public async Task<Comment?> FirstOrDefaultAsync(ISpecification<Comment> specification, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking();

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            query = specification.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CountAsync(ISpecification<Comment> specification, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking();

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            return await query.CountAsync(cancellationToken);
        }

        public async Task<bool> AnyAsync(ISpecification<Comment> specification, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking();

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            return await query.AnyAsync(cancellationToken);
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

            //query = query.Where(c => c.IsApproved).Cast<Comment>().AsQueryable();

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
