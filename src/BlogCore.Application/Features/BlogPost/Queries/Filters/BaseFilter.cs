using BlogCore.Core.Enums;
using MayFlo.Specification.Builder;

namespace BlogCore.Application.Features.BlogPost.Queries.Filters
{
    public static class BaseFilter
    {
        public static void ApplySearchFilter(SpecificationBuilder<Core.Entities.BlogPost> builder, string? searchTerm)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                builder.Where(b => b.Title.Contains(searchTerm))
                   .WhereOr(b => b.Excerpt.Contains(searchTerm))
                   .WhereOr(b => b.Content.Contains(searchTerm));
            }
        }
        public static void ApplyStatusFilter(SpecificationBuilder<Core.Entities.BlogPost> builder, PostStatus? status)
        {
            if (status.HasValue)
            {
                builder.Where(b => b.Status == status.Value);
            }
            else
            {
                // Default: exclude deleted posts unless specifically requested
                builder.Where(b => b.Status != PostStatus.Deleted);
            }
        }
        public static void ApplyCategoryFilter(SpecificationBuilder<Core.Entities.BlogPost> builder, Guid? categoryId)
        {
            if (categoryId.HasValue && categoryId.Value != Guid.Empty)
            {
                builder.Where(b => b.Categories.Any(c => c.Id == categoryId.Value));
            }
        }
        public static void ApplyTagFilter(SpecificationBuilder<Core.Entities.BlogPost> builder, Guid? tagId)
        {
            if (tagId.HasValue && tagId.Value != Guid.Empty)
            {
                builder.Where(b => b.Tags.Any(t => t.Id == tagId.Value));
            }
        }
        public static void ApplyDateRangeFilter(SpecificationBuilder<Core.Entities.BlogPost> builder, DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate.HasValue)
            {
                builder.Where(b => b.PublishedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                builder.Where(b => b.PublishedAt <= toDate.Value);
            }
        }
        public static void ApplySorting(SpecificationBuilder<Core.Entities.BlogPost> builder, string? sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                builder.OrderByDescending(b => b.CreatedAt);
                return;
            }

            switch (sortBy.ToLower())
            {
                case "title":
                    if (sortDescending)
                        builder.OrderByDescending(b => b.Title);
                    else
                        builder.OrderBy(b => b.Title);
                    break;
                case "viewcount":
                    if (sortDescending)
                        builder.OrderByDescending(b => b.ViewCount);
                    else
                        builder.OrderBy(b => b.ViewCount);
                    break;
                case "likecount":
                    if (sortDescending)
                        builder.OrderByDescending(b => b.LikeCount);
                    else
                        builder.OrderBy(b => b.LikeCount);
                    break;
                case "publishedat":
                    if (sortDescending)
                        builder.OrderByDescending(b => b.PublishedAt);
                    else
                        builder.OrderBy(b => b.PublishedAt);
                    break;
                case "createdat":
                    if (sortDescending)
                        builder.OrderByDescending(b => b.CreatedAt);
                    else
                        builder.OrderBy(b => b.CreatedAt);
                    break;
                default:
                    builder.OrderByDescending(b => b.CreatedAt);
                    break;
            }
        }
        public static void ApplyAuthorFilter(SpecificationBuilder<Core.Entities.BlogPost> builder, Guid? authorId)
        {
            if (authorId.HasValue && authorId.Value != Guid.Empty)
            {
                builder.Where(b => b.AuthorId == authorId.Value);
            }
        }
    }
}
