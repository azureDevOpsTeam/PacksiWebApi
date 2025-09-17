using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class ReferralStatusEnum : SmartEnum<ReferralStatusEnum>
{
    public static readonly ReferralStatusEnum Pending = new(1, "در انتظار", "Pending");
    public static readonly ReferralStatusEnum Completed = new(2, "تایید شده", "Completed");
    public static readonly ReferralStatusEnum Cancelled = new(3, "لغو شده", "Cancelled");

    public string PersianName { get; }

    public string EnglishName { get; }

    private ReferralStatusEnum(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}