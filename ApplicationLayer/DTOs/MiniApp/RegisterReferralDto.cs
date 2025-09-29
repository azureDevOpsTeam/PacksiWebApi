namespace ApplicationLayer.DTOs.MiniApp;

public class RegisterReferralDto
{
    public long TelegramUserId { get; set; }

    public long? ReferredByUserId { get; set; }

    public string ReferralCode { get; set; }

    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }
}