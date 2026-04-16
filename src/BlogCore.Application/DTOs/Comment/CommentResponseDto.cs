namespace BlogCore.Application.DTOs.Comment
{
    public class CommentResponseDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorEmail { get; set; } = string.Empty;
        public Guid BlogPostId { get; set; }
        public string BlogPostTitle { get; set; } = string.Empty;
        public Guid? ParentCommentId { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CommentResponseDto> Replies { get; set; } = new List<CommentResponseDto>();

        //public CommentResponseDto()
        //{
        //    Replies = new List<CommentResponseDto>();
        //}
    }
}
