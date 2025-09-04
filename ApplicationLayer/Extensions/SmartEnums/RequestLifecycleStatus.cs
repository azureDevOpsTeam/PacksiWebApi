using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class RequestLifecycleStatus : SmartEnum<RequestLifecycleStatus>
{
    public static readonly RequestLifecycleStatus Draft = new(1, "پیش نویس", nameof(Draft));
    public static readonly RequestLifecycleStatus Created = new(2, "ثبت اولیه (در انتظار بررسی مدیر)", nameof(Created));
    public static readonly RequestLifecycleStatus RejectedByManager = new(3, "رد شده توسط مدیر", nameof(RejectedByManager));
    public static readonly RequestLifecycleStatus Published = new(4, "منتشر شده", nameof(Published));
    public static readonly RequestLifecycleStatus FinalizateDelivery = new(5, "تحویل داده شده", nameof(FinalizateDelivery));
    public static readonly RequestLifecycleStatus NotDelivered = new(6, "تحویل نشده", nameof(NotDelivered));

    public string PersianName { get; }

    public string EnglishName { get; }

    private RequestLifecycleStatus(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}