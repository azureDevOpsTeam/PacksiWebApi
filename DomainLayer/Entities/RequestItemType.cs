using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class RequestItemType : BaseEntityModel, IAuditableEntity
    {
        public int RequestId { get; set; }

        public int ItemType { get; set; }

        public Request Request { get; set; }
    }
}