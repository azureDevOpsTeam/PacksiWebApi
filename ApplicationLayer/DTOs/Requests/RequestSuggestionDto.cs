namespace ApplicationLayer.DTOs.Requests;

public class RequestSuggestionDto
{
    public int RequestId { get; set; }

    public int RequestSuggestionId { get; set; }

    public decimal? SuggestionPrice { get; set; }

    public string Description { get; set; }
}