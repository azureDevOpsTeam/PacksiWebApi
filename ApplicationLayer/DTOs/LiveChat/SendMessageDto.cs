namespace ApplicationLayer.DTOs.LiveChat;

public class SendMessageDto
{
    public int ConversationId { get; set; }
    
    public int ReceiverId { get; set; }

    public string Content { get; set; }
}