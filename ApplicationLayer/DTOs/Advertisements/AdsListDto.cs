using ApplicationLayer.Extensions.SmartEnums;
using System.Text.Json.Serialization;

namespace ApplicationLayer.DTOs.Advertisements;

public class AdsListDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    [JsonIgnore]
    public int Status { get; set; }

    public string StatusEn => AdvertismentStatusEnum.FromValue(Status).EnglishName;

    public string StatusFa => AdvertismentStatusEnum.FromValue(Status).PersianName;

    [JsonIgnore]
    public int PostType { get; set; }

    public string PostTypeEn => AdsPostTypeEnum.FromValue(PostType).EnglishName;

    public string PostTypeFa => AdsPostTypeEnum.FromValue(PostType).PersianName;
}