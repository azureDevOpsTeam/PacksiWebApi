using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class TransportableItemTypeEnum : SmartEnum<TransportableItemTypeEnum>
{
    public static readonly TransportableItemTypeEnum Document = new(1, "مدرک", nameof(Document));
    public static readonly TransportableItemTypeEnum Pet = new(2, "پت", nameof(Pet));
    public static readonly TransportableItemTypeEnum Medicine = new(3, "دارو", nameof(Medicine));

    public string PersianName { get; }

    public string EnglishName { get; }

    private TransportableItemTypeEnum(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}