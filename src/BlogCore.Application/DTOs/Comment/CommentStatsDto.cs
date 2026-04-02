namespace BlogCore.Application.DTOs.Comment
{
    public class CommentStatsDto
    {
        public int TotalComments { get; set; }
        public int ApprovedComments { get; set; }
        public int PendingComments { get; set; }
        public int RejectedComments { get; set; }
        public int SpamComments { get; set; }
        public int AverageCommentsPerPost { get; set; }
    }
}
