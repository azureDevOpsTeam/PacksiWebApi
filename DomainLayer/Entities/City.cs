using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class City : BaseEntityModel, IAuditableEntity
    {
        public string PersianName { get; set; }

        public string Name { get; set; }

        public int CountryId { get; set; }

        public Country Country { get; set; }

        public ICollection<UserPreferredLocation> UserPreferredLocations { get; set; } = [];
    }
}