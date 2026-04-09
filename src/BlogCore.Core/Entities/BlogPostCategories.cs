using System.Text.Json.Serialization;

namespace BlogCore.Core.Entities
{
    public class BlogPostCategories
    {
        public Guid BlogPostId { get; set; }
        public Guid CategoryId { get; set; }

        [JsonIgnore]
        public virtual BlogPost BlogPost { get; set; }

        [JsonIgnore]
        public virtual Category Category { get; set; }
    }
}
