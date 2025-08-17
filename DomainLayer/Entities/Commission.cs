using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class Commission : BaseEntityModel, IAuditableEntity
    {
        public int ManagerId { get; set; }

        public int CarrierUserId { get; set; }

        public int RequestId { get; set; }

        public decimal Amount { get; set; }

        public UserAccount Manager { get; set; }

        public UserAccount CarrierUser { get; set; }

        public Request Request { get; set; }
    }
}