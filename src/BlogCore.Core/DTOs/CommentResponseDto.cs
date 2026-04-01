namespace BlogCore.Core.DTOs
{
    public class CommentResponseDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public Guid BlogPostId { get; set; }
        public string BlogPostTitle { get; set; }
        public Guid? ParentCommentId { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CommentResponseDto> Replies { get; set; }
    }
}
