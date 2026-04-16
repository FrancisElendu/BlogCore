using System.ComponentModel.DataAnnotations;

namespace BlogCore.Application.DTOs.Comment
{
    public class UpdateCommentDto
    {
        [StringLength(500, MinimumLength = 1)]
        public string Content { get; set; } = string.Empty;

        public bool? IsApproved { get; set; }
    }
}
