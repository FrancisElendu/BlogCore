namespace BlogCore.Core.DTOs
{
    public class ModerateCommentDto
    {
        public Guid CommentId { get; set; }
        public bool IsApproved { get; set; }
        public string ModerationNotes { get; set; }
    }
}
