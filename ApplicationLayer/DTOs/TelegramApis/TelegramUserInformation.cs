namespace ApplicationLayer.DTOs.TelegramApis
{
    public class TelegramUserInformationDto
    {
        public int TelegramUserId { get; set; }

        public string PhoneNumber { get; set; }

        public string UserName { get; set; }

        public int Language { get; set; }

        public bool IsPremium { get; set; }
    }
}