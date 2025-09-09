using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class CauseViolationEnum : SmartEnum<CauseViolationEnum>
{
    #region Fields

    public static CauseViolationEnum HighPrice = new(1, "قیمت بالا", nameof(HighPrice));
    public static CauseViolationEnum Insult = new(2, "توهین", nameof(Insult));
    public static CauseViolationEnum Dishonesty = new(3, "عدم امانتداری", nameof(Dishonesty));
    public static CauseViolationEnum NonDelivery = new(4, "عدم تحویل بار", nameof(NonDelivery));

    #endregion Fields

    public string PersianName { get; }

    public string EnglishName { get; }

    private CauseViolationEnum(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}