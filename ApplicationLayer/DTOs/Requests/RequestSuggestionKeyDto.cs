namespace ApplicationLayer.DTOs.Requests;

public class RequestSuggestionKeyDto
{
    public int RequestSuggestionId { get; set; }

    public int? DeliveryCode { get; set; }

    public int Rate { get; set; }
}