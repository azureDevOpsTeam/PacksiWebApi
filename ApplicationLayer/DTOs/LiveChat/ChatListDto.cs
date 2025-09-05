namespace ApplicationLayer.DTOs.LiveChat;

public class ChatListDto
{
    public int RequestId { get; set; }

    public int RequestCreatorId { get; set; }

    public string RequestCreatorDisplayName { get; set; }

    public int CurrentUserAccountId { get; set; }

    public string Avatar { get; set; }

    public string LastMessage { get; set; }

    public bool IsOnline { get; set; }

    public bool IsBlocked { get; set; }

    public DateTime? LastSeen { get; set; }
}