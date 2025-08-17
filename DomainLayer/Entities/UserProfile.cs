using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class UserProfile : BaseEntityModel, IAuditableEntity
    {
        public int UserAccountId { get; set; }

        public int? CountryOfResidenceId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DisplayName { get; set; }

        public string Address { get; set; }

        public string Company { get; set; }

        public string PostalCode { get; set; }

        public int? Gender { get; set; }

        public int? MaritalStatus { get; set; }

        public Country Country { get; set; }

        public UserAccount UserAccount { get; set; }
    }
}