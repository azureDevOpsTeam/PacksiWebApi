using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class TransportableItemTypeEnum : SmartEnum<TransportableItemTypeEnum>
{
    public static readonly TransportableItemTypeEnum Document = new(1, "مدرک", nameof(Document));
    public static readonly TransportableItemTypeEnum Cat = new(2, "پیشی", nameof(Cat));
    public static readonly TransportableItemTypeEnum Dog = new(3, "هاپو", nameof(Dog));
    public static readonly TransportableItemTypeEnum Medicine = new(4, "دارو", nameof(Medicine));

    public string PersianName { get; }

    public string EnglishName { get; }

    private TransportableItemTypeEnum(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}