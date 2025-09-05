namespace ApplicationLayer.DTOs.LiveChat;

public class ChatListDto
{
    public int RequestCreatorId { get; set; }

    public string RequestCreatorDisplayName { get; set; }

    public int CurrentUserAccountId { get; set; }

    public bool IsOnline { get; set; }

    public bool IsBlocked { get; set; }

    public DateTime? LastSeen { get; set; }
}