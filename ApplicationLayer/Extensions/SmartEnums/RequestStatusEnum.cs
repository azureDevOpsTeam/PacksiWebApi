using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class RequestStatusEnum : SmartEnum<RequestStatusEnum>
{
    public static readonly RequestStatusEnum Created = new(1, "ثبت اولیه (در انتظار بررسی مدیر)", nameof(Created));
    public static readonly RequestStatusEnum RejectedByManager = new(2, "رد شده توسط مدیر", nameof(RejectedByManager));
    public static readonly RequestStatusEnum Published = new(3, "منتشر شده", nameof(Published));
    public static readonly RequestStatusEnum Selected = new(4, "انتخاب شده", nameof(Selected));
    public static readonly RequestStatusEnum RejectedBySender = new(5, "رد شده توسط ارسال‌کننده", nameof(RejectedBySender));
    public static readonly RequestStatusEnum ConfirmedBySender = new(6, "تایید شده توسط ارسال‌کننده", nameof(ConfirmedBySender));
    public static readonly RequestStatusEnum ReadyToPickup = new(7, "آماده دریافت بار", nameof(ReadyToPickup));
    public static readonly RequestStatusEnum PickedUp = new(8, "تحویل گرفته شده", nameof(PickedUp));
    public static readonly RequestStatusEnum InTransit = new(9, "در حال انتقال", nameof(InTransit));
    public static readonly RequestStatusEnum ReadyToDeliver = new(10, "آماده تحویل", nameof(ReadyToDeliver));
    public static readonly RequestStatusEnum Delivered = new(11, "تحویل داده شده", nameof(Delivered));
    public static readonly RequestStatusEnum NotDelivered = new(12, "تحویل داده نشد", nameof(NotDelivered));

    public string PersianName { get; }

    public string EnglishName { get; }

    private RequestStatusEnum(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}