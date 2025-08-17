using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.DTOs.Requests;

public class UpdateRequestDto
{
    public int RequestId { get; set; }

    public int OriginCityId { get; set; }

    public int DestinationCityId { get; set; }

    public DateTime DepartureDate { get; set; }

    public DateTime ArrivalDate { get; set; }

    public int RequestType { get; set; }

    public decimal? SuggestedPrice { get; set; }

    public string Description { get; set; }

    public double? MaxWeightKg { get; set; }

    public double? MaxLengthCm { get; set; }

    public double? MaxWidthCm { get; set; }

    public double? MaxHeightCm { get; set; }

    //public int MainOriginCityId { get; set; }

    //public int MainDestinationCityId { get; set; }

    //public int AttachmentType { get; set; }

    public List<int> ItemTypeIds { get; set; } = [];

    public List<IFormFile> Files { get; set; } = new();

    public List<DeliverableOriginLocationDto> AvailableOrigins { get; set; } = [];

    public List<DeliverableDestinationLocationDto> AvailableDestinations { get; set; } = [];

    //public List<CreateRequestAttachmentDto> Attachments { get; set; } = [];
}