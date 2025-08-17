using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class DayOfWeekEnum : SmartEnum<DayOfWeekEnum>
{
    public static readonly DayOfWeekEnum Sunday = new(1, "یک‌شنبه", "Sunday");
    public static readonly DayOfWeekEnum Monday = new(2, "دوشنبه", "Monday");
    public static readonly DayOfWeekEnum Tuesday = new(3, "سه‌شنبه", "Tuesday");
    public static readonly DayOfWeekEnum Wednesday = new(4, "چهارشنبه", "Wednesday");
    public static readonly DayOfWeekEnum Thursday = new(5, "پنج‌شنبه", "Thursday");
    public static readonly DayOfWeekEnum Friday = new(6, "جمعه", "Friday");
    public static readonly DayOfWeekEnum Saturday = new(7, "شنبه", "Saturday");

    public string PersianName { get; }

    public string EnglishName { get; }

    private DayOfWeekEnum(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}