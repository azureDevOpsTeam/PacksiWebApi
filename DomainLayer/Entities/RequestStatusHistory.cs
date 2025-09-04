using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class RequestStatusHistory : BaseEntityModel, IAuditableEntity
    {
        public int? UserAccountId { get; set; }

        public int RequestSelectionId { get; set; }

        public int Status { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        public string Comment { get; set; }

        public UserAccount UserAccount { get; set; }

        public RequestSelection RequestSelection { get; set; }
    }
}