using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class Invitation : BaseEntityModel, IAuditableEntity
    {
        public string Code { get; set; } = Guid.NewGuid().ToString("N")[..10];

        public int InviterUserId { get; set; }

        public UserAccount InviterUser { get; set; }

        public int UsageCount { get; set; } = 0;

        public int? MaxUsageCount { get; set; }

        public DateTime? ExpireDate { get; set; }
    }
}