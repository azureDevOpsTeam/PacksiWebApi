using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class RequestProcessStatus : SmartEnum<RequestProcessStatus>
{
    public static readonly RequestProcessStatus Published = new(100, "منتشر شده", nameof(Published));
    public static readonly RequestProcessStatus Selected = new(101, "انتخاب شده", nameof(Selected));
    public static readonly RequestProcessStatus RejectedBySender = new(102, "رد شده توسط ارسال‌کننده", nameof(RejectedBySender));
    public static readonly RequestProcessStatus ConfirmedBySender = new(103, "تایید شده توسط ارسال‌کننده", nameof(ConfirmedBySender));
    public static readonly RequestProcessStatus ReadyToPickup = new(104, "آماده دریافت بار", nameof(ReadyToPickup));
    public static readonly RequestProcessStatus PickedUp = new(105, "تحویل گرفته شده", nameof(PickedUp));
    public static readonly RequestProcessStatus InTransit = new(106, "در حال انتقال", nameof(InTransit));
    public static readonly RequestProcessStatus ReadyToDeliver = new(107, "آماده تحویل", nameof(ReadyToDeliver));
    public static readonly RequestProcessStatus Delivered = new(108, "تحویل داده شده", nameof(Delivered));
    public static readonly RequestProcessStatus NotDelivered = new(109, "تحویل داده نشد", nameof(NotDelivered));

    public string PersianName { get; }

    public string EnglishName { get; }

    private RequestProcessStatus(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}