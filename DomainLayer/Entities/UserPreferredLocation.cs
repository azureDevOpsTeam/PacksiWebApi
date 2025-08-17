using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class UserPreferredLocation : BaseEntityModel, IAuditableEntity
    {
        public int UserAccountId { get; set; }

        public int? CountryId { get; set; }

        public int? CityId { get; set; }

        public UserAccount UserAccount { get; set; }

        public Country Country { get; set; }

        public City City { get; set; }
    }
}