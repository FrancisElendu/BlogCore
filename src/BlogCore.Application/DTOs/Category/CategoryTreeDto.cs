namespace BlogCore.Application.DTOs.Category
{
    public class CategoryTreeDto
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public bool HasChildren { get; set; }
        public List<CategoryTreeDto> Children { get; set; }

        public CategoryTreeDto()
        {
            Children = new List<CategoryTreeDto>();
        }
    }
}
