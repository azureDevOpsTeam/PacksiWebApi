using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class AdvertisementTypeEnum : SmartEnum<AdvertisementTypeEnum>
{
    public static readonly AdvertisementTypeEnum ByViewCount = new(1, "تعداد بازدید", nameof(ByViewCount));
    public static readonly AdvertisementTypeEnum ByTimeSlot = new(2, "نمایش ساعتی", nameof(ByTimeSlot));

    public string PersianName { get; }

    public string EnglishName { get; }

    private AdvertisementTypeEnum(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}