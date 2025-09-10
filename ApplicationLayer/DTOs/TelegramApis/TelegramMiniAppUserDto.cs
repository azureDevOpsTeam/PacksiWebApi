namespace ApplicationLayer.DTOs.TelegramApis
{
    /// <summary>
    /// DTO برای اطلاعات کاربر Telegram MiniApp
    /// </summary>
    public class TelegramMiniAppUserDto
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string LanguageCode { get; set; }

        public bool IsPremium { get; set; }

        public bool AllowsWriteToPm { get; set; }

        public string PhotoUrl { get; set; }

        public int? ReferredByUserId { get; set; }
    }

    /// <summary>
    /// DTO برای نتیجه اعتبارسنجی Telegram MiniApp
    /// </summary>
    public class TelegramMiniAppValidationResultDto
    {
        public bool IsValid { get; set; }

        public bool ExistUser { get; set; }

        public TelegramMiniAppUserDto User { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime AuthDate { get; set; }

        public string Hash { get; set; }

        public string StartParam { get; set; }
    }

    /// <summary>
    /// DTO برای داده‌های اولیه Telegram MiniApp
    /// </summary>
    public class TelegramInitDataDto
    {
        public string QueryId { get; set; }

        public TelegramMiniAppUserDto User { get; set; }

        public string Receiver { get; set; }

        public string ChatType { get; set; }

        public string ChatInstance { get; set; }

        public string StartParam { get; set; }

        public long AuthDate { get; set; }

        public string Hash { get; set; }
    }
}