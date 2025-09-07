using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class Request : BaseEntityModel, IAuditableEntity
    {
        public int UserAccountId { get; set; }

        public int? OriginCityId { get; set; }

        public int? DestinationCityId { get; set; }

        public DateTime DepartureDate { get; set; }

        public DateTime ArrivalDate { get; set; }

        public int RequestType { get; set; }

        public decimal? SuggestedPrice { get; set; }

        public string Description { get; set; }

        public double? MaxWeightKg { get; set; }

        public double? MaxLengthCm { get; set; }

        public double? MaxWidthCm { get; set; }

        public double? MaxHeightCm { get; set; }

        public int Status { get; set; }

        public UserAccount UserAccount { get; set; }

        public City OriginCity { get; set; }

        public City DestinationCity { get; set; }

        public ICollection<RequestItemType> RequestItemTypes { get; set; } = [];

        public ICollection<RequestAttachment> Attachments { get; set; } = [];

        public ICollection<RequestAvailableOrigin> AvailableOrigins { get; set; } = [];

        public ICollection<RequestAvailableDestination> AvailableDestinations { get; set; } = [];

        public ICollection<Suggestion> Suggestions { get; set; } = [];
    }
}