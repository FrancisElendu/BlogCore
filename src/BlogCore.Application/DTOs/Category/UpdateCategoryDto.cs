using System.ComponentModel.DataAnnotations;

namespace BlogCore.Application.DTOs.Category
{
    public class UpdateCategoryDto
    {
        public Guid Id { get; set; }

        public string Slug { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;
    }
}
