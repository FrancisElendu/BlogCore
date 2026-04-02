namespace BlogCore.Application.DTOs.Category
{
    public class CategoryResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }

        // Parent category information
        public Guid? ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }

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
