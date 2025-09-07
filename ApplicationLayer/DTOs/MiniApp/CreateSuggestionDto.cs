namespace ApplicationLayer.DTOs.MiniApp;

public class CreateSuggestionDto
{
    public int RequestId { get; set; }

    public decimal SuggestionPrice { get; set; }

    public int Currency { get; set; }

    public string Description { get; set; }
}