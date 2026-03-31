using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.Core.Entities
{
    public class BlogPostTags
    {
        public Guid BlogPostId { get; set; }
        public Guid TagId { get; set; }

        public virtual BlogPost BlogPost { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
