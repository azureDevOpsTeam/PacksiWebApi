namespace ApplicationLayer.DTOs.TelegramApis
{
    public class TelegramInfoDto
    {
        public bool UserExists { get; set; }

        public long TelegramUserId { get; set; }

        public string PhoneNumber { get; set; }

        public int Language { get; set; }
    }
}