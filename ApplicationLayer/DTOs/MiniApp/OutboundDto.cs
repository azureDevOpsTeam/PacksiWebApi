namespace ApplicationLayer.DTOs.MiniApp;

public class OutboundDto
{
    public int RequestId { get; set; }

    public int UserAccountId { get; set; }

    public string OriginCity { get; set; }

    public string DestinationCity { get; set; }

    public DateTime DepartureDate { get; set; }

    public string DepartureDatePersian { get; set; }

    public DateTime ArrivalDate { get; set; }

    public string ArrivalDatePersian { get; set; }

    public decimal? SuggestedPrice { get; set; }

    public string Description { get; set; }

    public double? MaxWeightKg { get; set; }

    public double? MaxLengthCm { get; set; }

    public double? MaxWidthCm { get; set; }

    public double? MaxHeightCm { get; set; }
}