namespace BlogCore.Application.DTOs.Tag
{
    public  class BulkTagResultDto
    {
        public List<TagSummaryDto> CreatedTags { get; set; }
        public List<TagSummaryDto> ExistingTags { get; set; }
        public List<string> FailedTags { get; set; }
        public int TotalProcessed { get; set; }
        public int TotalCreated { get; set; }
        public int TotalExisting { get; set; }

        public BulkTagResultDto()
        {
            CreatedTags = new List<TagSummaryDto>();
            ExistingTags = new List<TagSummaryDto>();
            FailedTags = new List<string>();
        }
    }
}
