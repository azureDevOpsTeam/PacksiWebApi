namespace ApplicationLayer.DTOs.Dashboard;

public class InboundOutboundTripsDto
{
    public List<TripDto> InboundTrips { get; set; } = new();

    public List<TripDto> OutboundTrips { get; set; } = new();
}