namespace ApplicationLayer.DTOs.LiveChat;

public class SendMessageDto
{
    public int ReceiverId { get; set; }

    public string Content { get; set; }
}