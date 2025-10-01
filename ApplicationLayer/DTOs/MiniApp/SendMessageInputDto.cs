namespace ApplicationLayer.DTOs.MiniApp;

public class SendMessageInputDto
{
    public long TelegramUserId { get; set; }

    public string Message { get; set; }
}
