namespace BlogCore.Core.Entities
{
    public class BlogPostCategories
    {
        public Guid BlogPostId { get; set; }
        public Guid CategoryId { get; set; }

        public virtual BlogPost BlogPost { get; set; }
        public virtual Category Category { get; set; }
    }
}
