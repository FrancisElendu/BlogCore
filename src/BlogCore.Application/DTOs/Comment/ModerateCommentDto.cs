namespace BlogCore.Application.DTOs.Comment
{
    public class ModerateCommentDto
    {
        public Guid CommentId { get; set; }
        public bool IsApproved { get; set; }
        public string ModerationNotes { get; set; }
    }
}
