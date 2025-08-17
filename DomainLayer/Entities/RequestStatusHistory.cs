using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class RequestStatusHistory : BaseEntityModel, IAuditableEntity
    {
        public int? UserAccountId { get; set; }

        public int RequestId { get; set; }

        public int Status { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        public string Comment { get; set; }

        public UserAccount ChangedByUser { get; set; }

        public Request Request { get; set; }
    }
}