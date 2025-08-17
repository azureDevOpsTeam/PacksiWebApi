namespace ApplicationLayer.DTOs.Comments;

public class CommentDetailDto
{
    public int Id { get; set; }

    public string Content { get; set; } = default!;

    public bool IsApproved { get; set; }

    public int UserAccountId { get; set; }

    public string DisplayName { get; set; } = default!;

    public string PhoneNumber { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
}