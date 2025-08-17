using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class AttachmentTypeEnum : SmartEnum<AttachmentTypeEnum>
{
    public static readonly AttachmentTypeEnum Ticket = new(1, "بلیط سفر", nameof(Ticket));
    public static readonly AttachmentTypeEnum IdentityDocument = new(2, "مدرک هویتی", nameof(IdentityDocument));
    public static readonly AttachmentTypeEnum ItemImage = new(3, "تصویر بار", nameof(ItemImage));

    public string PersianName { get; }

    public string EnglishName { get; }

    private AttachmentTypeEnum(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}