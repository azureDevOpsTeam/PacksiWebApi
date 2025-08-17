using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class TelegramPostLog : BaseEntityModel, IAuditableEntity
    {
        public int AdvertisementId { get; set; }

        public DateTime SentAt { get; set; }

        public bool IsSuccessful { get; set; }

        public string TelegramMessageId { get; set; }

        public string ErrorMessage { get; set; }

        public Advertisement Advertisement { get; set; }
    }
}