using BlogCore.Application.DTOs.BlogPost;
using BlogCore.Application.DTOs.Category;
using BlogCore.Application.DTOs.Comment;
using BlogCore.Application.DTOs.Tag;
using BlogCore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.Application.Common.Mappings
{
    public static class ManualMapper
    {
        // BlogPost mappings
        public static BlogPostResponseDto MapToBlogPostResponseDto(BlogPost entity)
        {
            if (entity == null) return null!;

            return new BlogPostResponseDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Slug = entity.Slug,
                Excerpt = entity.Excerpt,
                Content = entity.Content,
                FeaturedImageUrl = entity.FeaturedImageUrl,
                Status = entity.Status,
                ViewCount = entity.ViewCount,
                LikeCount = entity.LikeCount,
                AuthorId = entity.AuthorId,
                AuthorName = string.Empty, // Will be set from User service
                Categories = entity.Categories?.Select(MapToCategoryDto).ToList() ?? new List<CategoryDto>(),
                Tags = entity.Tags?.Select(MapToTagDto).ToList() ?? new List<TagDto>(),
                PublishedAt = entity.PublishedAt,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CommentCount = entity.Comments?.Count(c => c.IsApproved) ?? 0,
                RecentComments = entity.Comments?
                    .Where(c => c.IsApproved)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(5)
                    .Select(MapToCommentResponseDto)
                    .ToList() ?? new List<CommentResponseDto>()
            };
        }

        public static BlogPostSummaryDto MapToBlogPostSummaryDto(BlogPost entity)
        {
            if (entity == null) return null!;

            return new BlogPostSummaryDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Slug = entity.Slug,
                Excerpt = entity.Excerpt,
                PublishedAt = entity.PublishedAt,
                ViewCount = entity.ViewCount,
                LikeCount = entity.LikeCount,
                Summary = entity.Excerpt?.Length > 200
                    ? entity.Excerpt.Substring(0, 200) + "..."
                    : entity.Excerpt ?? string.Empty
            };
        }

        public static List<BlogPostSummaryDto> MapToBlogPostSummaryDtoList(IEnumerable<BlogPost> entities)
        {
            return entities?.Select(MapToBlogPostSummaryDto).ToList() ?? new List<BlogPostSummaryDto>();
        }

        // Category mappings
        public static CategoryDto MapToCategoryDto(Category entity)
        {
            if (entity == null) return null!;

            return new CategoryDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Slug = entity.Slug
            };
        }

        public static CategoryResponseDto MapToCategoryResponseDto(Category entity)
        {
            if (entity == null) return null!;

            return new CategoryResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Slug = entity.Slug,
                ParentCategoryId = entity.ParentCategoryId,
                ParentCategoryName = entity.ParentCategory?.Name ?? string.Empty,
                BlogPostCount = entity.BlogPosts?.Count ?? 0,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                SubCategories = entity.SubCategories?.Select(MapToCategoryResponseDto).ToList() ?? new List<CategoryResponseDto>()
            };
        }

        public static CategorySummaryDto MapToCategorySummaryDto(Category entity)
        {
            if (entity == null) return null!;

            return new CategorySummaryDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Slug = entity.Slug,
                BlogPostCount = entity.BlogPosts?.Count ?? 0
            };
        }

        public static CategoryTreeDto MapToCategoryTreeDto(Category entity, int level = 0)
        {
            if (entity == null) return null!;

            return new CategoryTreeDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Slug = entity.Slug,
                Description = entity.Description,
                Level = level,
                HasChildren = entity.SubCategories?.Any() ?? false,
                Children = entity.SubCategories?.Select(c => MapToCategoryTreeDto(c, level + 1)).ToList() ?? new List<CategoryTreeDto>()
            };
        }

        // Tag mappings
        public static TagDto MapToTagDto(Tag entity)
        {
            if (entity == null) return null!;

            return new TagDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Slug = entity.Slug
            };
        }

        public static TagSummaryDto MapToTagSummaryDto(Tag entity)
        {
            if (entity == null) return null!;

            return new TagSummaryDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Slug = entity.Slug,
                BlogPostCount = entity.BlogPosts?.Count ?? 0
            };
        }

        public static TagResponseDto MapToTagResponseDto(Tag entity)
        {
            if (entity == null) return null!;

            return new TagResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Slug = entity.Slug,
                BlogPostCount = entity.BlogPosts?.Count ?? 0,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                RecentPosts = entity.BlogPosts?
                    .OrderByDescending(bp => bp.PublishedAt)
                    .Take(5)
                    .Select(MapToTagBlogPostSummaryDto)
                    .ToList() ?? new List<TagBlogPostSummaryDto>()
            };
        }

        public static TagCloudDto MapToTagCloudDto(Tag entity, int maxCount)
        {
            if (entity == null) return null!;

            var postCount = entity.BlogPosts?.Count ?? 0;
            var weight = maxCount > 0 ? (int)Math.Ceiling((double)postCount / maxCount * 10) : 1;

            return new TagCloudDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Slug = entity.Slug,
                BlogPostCount = postCount,
                Weight = weight,
                SizeClass = GetSizeClass(weight)
            };
        }

        public static TagBlogPostSummaryDto MapToTagBlogPostSummaryDto(BlogPost entity)
        {
            if (entity == null) return null!;

            return new TagBlogPostSummaryDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Slug = entity.Slug,
                Excerpt = entity.Excerpt,
                PublishedAt = entity.PublishedAt
            };
        }

        public static TagWithPostsDto MapToTagWithPostsDto(Tag entity, List<BlogPostSummaryDto> posts, int pageNumber, int pageSize, int totalCount)
        {
            if (entity == null) return null!;

            return new TagWithPostsDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Slug = entity.Slug,
                BlogPostCount = entity.BlogPosts?.Count ?? 0,
                BlogPosts = posts,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                TotalCount = totalCount
            };
        }

        // Comment mappings
        public static CommentResponseDto MapToCommentResponseDto(Comment entity)
        {
            if (entity == null) return null!;

            return new CommentResponseDto
            {
                Id = entity.Id,
                Content = entity.Content,
                AuthorName = entity.AuthorName,
                AuthorEmail = entity.AuthorEmail,
                BlogPostId = entity.BlogPostId,
                BlogPostTitle = entity.BlogPost?.Title ?? string.Empty,
                ParentCommentId = entity.ParentCommentId,
                IsApproved = entity.IsApproved,
                CreatedAt = entity.CreatedAt,
                Replies = entity.Replies?
                    .Where(r => r.IsApproved)
                    .Select(MapToCommentResponseDto)
                    .ToList() ?? new List<CommentResponseDto>()
            };
        }

        public static CommentStatsDto MapToCommentStatsDto(int total, int approved, int pending, int rejected, int spam, int avgPerPost)
        {
            return new CommentStatsDto
            {
                TotalComments = total,
                ApprovedComments = approved,
                PendingComments = pending,
                RejectedComments = rejected,
                SpamComments = spam,
                AverageCommentsPerPost = avgPerPost
            };
        }

        // Bulk mapping methods
        public static List<TDestination> MapList<TSource, TDestination>(
            IEnumerable<TSource> source,
            Func<TSource, TDestination> mapFunc)
        {
            return source?.Select(mapFunc).ToList() ?? new List<TDestination>();
        }

        private static string GetSizeClass(int weight)
        {
            return weight switch
            {
                >= 8 => "tag-xl",
                >= 6 => "tag-lg",
                >= 4 => "tag-md",
                >= 2 => "tag-sm",
                _ => "tag-xs"
            };
        }
    }
}
