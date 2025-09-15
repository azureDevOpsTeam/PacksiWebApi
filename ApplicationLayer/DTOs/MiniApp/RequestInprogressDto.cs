namespace ApplicationLayer.DTOs.MiniApp;

public class RequestInprogressDto
{
    public List<RequestInfoDto> MyReciveOffers { get; set; }

    public List<RequestInfoDto> MySentOffers { get; set; }
}

public class RequestInfoDto
{
    public int Id { get; set; }

    public string OriginCityName { get; set; }

    public string OriginCityPersianName { get; set; }

    public string DestinationCityName { get; set; }

    public string DestinationCityPersianName { get; set; }

    public int Status { get; set; }

    public List<ActiveSuggestionDto> Suggestions { get; set; } = new();
}

public class ActiveSuggestionDto
{
    public int Id { get; set; }

    public string DisplayName { get; set; }

    public decimal SuggestionPrice { get; set; }

    public int Currency { get; set; }

    public int ItemType { get; set; }

    public DateTime CreatedOn { get; set; }

    public List<string> Attachments { get; set; } = new();
}