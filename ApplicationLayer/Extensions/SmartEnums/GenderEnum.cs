using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class GenderEnum : SmartEnum<GenderEnum>
{
    public static readonly GenderEnum Male = new(1, "آقا", "Male");
    public static readonly GenderEnum Female = new(2, "خانم", "Female");

    public string PersianName { get; }

    public string EnglishName { get; }

    private GenderEnum(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}