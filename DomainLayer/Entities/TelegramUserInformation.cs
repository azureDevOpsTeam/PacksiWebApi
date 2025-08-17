using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class TelegramUserInformation : BaseEntityModel, IAuditableEntity
    {
        public int UserAccountId { get; set; }

        public int TelegramUserId { get; set; }

        public string UserName { get; set; }

        public int Language { get; set; }

        public bool IsPremium { get; set; }

        public UserAccount UserAccount { get; set; }
    }
}