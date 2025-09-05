namespace ApplicationLayer.DTOs.LiveChat;

public class ChatListDto
{
    public int ConversationId { get; set; }

    public int RequestId { get; set; }

    public int ReciverId { get; set; }

    public string RequestCreatorDisplayName { get; set; }

    public int SenderId { get; set; }

    public string Avatar { get; set; }

    public string LastMessage { get; set; }

    public bool IsOnline { get; set; }

    public bool IsBlocked { get; set; }

    public string LastSeenEn { get; set; }

    public string LastSeenFa { get; set; }
}