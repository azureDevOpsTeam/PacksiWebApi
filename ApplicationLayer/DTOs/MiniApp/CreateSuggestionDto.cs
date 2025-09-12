using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.DTOs.MiniApp;

public class CreateSuggestionDto
{
    public int RequestId { get; set; }

    public decimal SuggestionPrice { get; set; }

    public int Currency { get; set; }

    public string Description { get; set; }

    public int ItemTypeId { get; set; }

    public List<IFormFile> Files { get; set; } = new();
}