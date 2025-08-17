using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class PostPricing : BaseEntityModel, IAuditableEntity
    {
        public int PostType { get; set; }

        public int AdvertisementType { get; set; }

        public decimal Price { get; set; }
    }
}