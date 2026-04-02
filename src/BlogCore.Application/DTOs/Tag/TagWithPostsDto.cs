using BlogCore.Application.DTOs.BlogPost;

namespace BlogCore.Application.DTOs.Tag
{
    public class TagWithPostsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int BlogPostCount { get; set; }

        // Paginated posts using this tag
        public List<BlogPostSummaryDto> BlogPosts { get; set; }

        // Pagination metadata
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public TagWithPostsDto()
        {
            BlogPosts = new List<BlogPostSummaryDto>();
        }
    }
}
