namespace ApplicationLayer.DTOs.Dashboard;

public class ReportTripsDto
{
    public int CarryerOutboundTrips { get; set; }

    public int CarryerInboundTrips { get; set; }

    public int SenderInboundTrips { get; set; }

    public int SenderOutboundTrips { get; set; }
}