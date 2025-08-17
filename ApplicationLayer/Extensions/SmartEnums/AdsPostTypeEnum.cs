using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class AdsPostTypeEnum : SmartEnum<AdsPostTypeEnum>
{
    public static readonly AdsPostTypeEnum Text = new(1, "متن", nameof(Text));
    public static readonly AdsPostTypeEnum Image = new(2, "تصویر", nameof(Image));
    public static readonly AdsPostTypeEnum Video = new(3, "ویدیو", nameof(Video));

    public string PersianName { get; }

    public string EnglishName { get; }

    private AdsPostTypeEnum(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}