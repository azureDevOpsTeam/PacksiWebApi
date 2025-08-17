using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class RequestAvailableDestination : BaseEntityModel, IAuditableEntity
    {
        public int RequestId { get; set; }

        public int? CityId { get; set; }

        public string LocationDescription { get; set; }

        public City City { get; set; }

        public Request Request { get; set; }
    }
}