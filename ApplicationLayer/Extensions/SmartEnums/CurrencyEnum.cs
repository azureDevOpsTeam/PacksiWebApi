using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class CurrencyEnum : SmartEnum<CurrencyEnum>
{
    public static readonly CurrencyEnum USDT = new(1, "دلار", "USDT");
    public static readonly CurrencyEnum IRR = new(2, "ریال", "IRR");

    public string PersianName { get; }

    public string EnglishName { get; }

    private CurrencyEnum(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}