using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.Application.DTOs.BlogPost
{
    public class BlogPostSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public DateTime PublishedAt { get; set; }
        public string Slug { get; set; }
        public string Excerpt { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
    }
}
