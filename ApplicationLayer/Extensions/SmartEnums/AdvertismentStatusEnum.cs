using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class AdvertismentStatusEnum : SmartEnum<AdvertismentStatusEnum>
{
    public static readonly AdvertismentStatusEnum Created = new(1, "ثبت اولیه", nameof(Created));
    public static readonly AdvertismentStatusEnum Rejected = new(2, "رد شده", nameof(Rejected));
    public static readonly AdvertismentStatusEnum AwaitingPayment = new(3, "در انتظار پرداخت", nameof(AwaitingPayment));
    public static readonly AdvertismentStatusEnum Paid = new(4, "پرداخت شده", nameof(Paid));
    public static readonly AdvertismentStatusEnum Published = new(3, "منتشر شده", nameof(Published));

    public string PersianName { get; }

    public string EnglishName { get; }

    private AdvertismentStatusEnum(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}