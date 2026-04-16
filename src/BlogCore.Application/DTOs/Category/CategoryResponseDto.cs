namespace BlogCore.Application.DTOs.Category
{
    public class CategoryResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        // Parent category information
        public Guid? ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; } = string.Empty;

        // Child categories (for hierarchical display)
        public List<CategoryResponseDto> SubCategories { get; set; }

        // Statistics
        public int BlogPostCount { get; set; }

        // Audit information
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public CategoryResponseDto()
        {
            SubCategories = new List<CategoryResponseDto>();
        }
    }
}
