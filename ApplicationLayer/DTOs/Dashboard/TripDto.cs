namespace ApplicationLayer.DTOs.Dashboard;
public class TripDto
{
    public DateTime DepartureDate { get; set; }

    public List<string> ItemTypes { get; set; } = new();
}