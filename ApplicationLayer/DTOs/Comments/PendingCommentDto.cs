namespace ApplicationLayer.DTOs.Comments;

public class PendingCommentDto
{
    public int Id { get; set; }

    public string Content { get; set; } = default!;

    public string DisplayName { get; set; } = default!;

    public string PhoneNumber { get; set; } = default!;

    public string CreatedAt { get; set; }
}