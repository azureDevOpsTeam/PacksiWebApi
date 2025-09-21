namespace ApplicationLayer.DTOs.Requests;

public class AddRatingDto
{
    public int RequestSuggestionId { get; set; }

    public int Rate { get; set; }
}