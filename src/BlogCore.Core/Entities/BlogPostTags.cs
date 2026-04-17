using System.Text.Json.Serialization;

namespace BlogCore.Core.Entities
{
    public class BlogPostTags
    {
        public Guid BlogPostId { get; set; }
        public Guid TagId { get; set; }

        [JsonIgnore]
        public virtual BlogPost? BlogPost { get; set; } = null;

        [JsonIgnore]
        public virtual Tag? Tag { get; set; } = null;
    }
}
