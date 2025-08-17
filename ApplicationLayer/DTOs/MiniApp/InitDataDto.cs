using System.Text.Json.Serialization;

namespace ApplicationLayer.DTOs.MiniApp;

public class InitDataDto
{
    [JsonIgnore]
    public string InitData { get; set; }

    [JsonIgnore]
    public string Platform { get; set; }
}