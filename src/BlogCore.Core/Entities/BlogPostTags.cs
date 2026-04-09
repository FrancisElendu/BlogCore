using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlogCore.Core.Entities
{
    public class BlogPostTags
    {
        public Guid BlogPostId { get; set; }
        public Guid TagId { get; set; }

        [JsonIgnore]
        public virtual BlogPost BlogPost { get; set; }

        [JsonIgnore]
        public virtual Tag Tag { get; set; }
    }
}
