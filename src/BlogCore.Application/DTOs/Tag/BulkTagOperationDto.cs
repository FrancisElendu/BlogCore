using System.ComponentModel.DataAnnotations;

namespace BlogCore.Application.DTOs.Tag
{
    public class BulkTagOperationDto
    {
        [Required]
        public List<string> TagNames { get; set; } = new List<string>();

        public bool CreateIfNotExists { get; set; } = true;
    }
}
