using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class Country : BaseEntityModel, IAuditableEntity
    {
        public string Flag { get; set; }

        public string PersianName { get; set; }

        public string Name { get; set; }

        public string PhonePrefix { get; set; }

        public string IsoCode { get; set; }

        public ICollection<City> Cities { get; set; } = [];

        public ICollection<UserProfile> UserProfiles { get; set; } = [];

        public ICollection<UserPreferredLocation> UserPreferredLocations { get; set; } = [];
    }
}