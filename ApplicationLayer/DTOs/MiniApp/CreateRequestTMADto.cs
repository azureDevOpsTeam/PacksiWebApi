using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.DTOs.MiniApp;

public class CreateRequestTMADto
{
    public int OriginCityId { get; set; }

    public int DestinationCityId { get; set; }

    public DateTime DepartureDate { get; set; }

    public DateTime ArrivalDate { get; set; }

    public int RequestType { get; set; }

    public string Description { get; set; }

    public double? MaxWeightKg { get; set; }

    public double? MaxLengthCm { get; set; }

    public double? MaxWidthCm { get; set; }

    public double? MaxHeightCm { get; set; }

    public List<int> ItemTypeIds { get; set; } = [];

    public List<IFormFile> Files { get; set; } = new();
}