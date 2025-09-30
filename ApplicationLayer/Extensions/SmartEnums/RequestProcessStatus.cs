using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class RequestProcessStatus : SmartEnum<RequestProcessStatus>
{
    public static readonly RequestProcessStatus Published = new(100, "منتشر شده", nameof(Published));
    public static readonly RequestProcessStatus Selected = new(101, "انتخاب شده", nameof(Selected));
    public static readonly RequestProcessStatus RejectedBySender = new(102, "رد شده توسط ارسال‌کننده", nameof(RejectedBySender));
    public static readonly RequestProcessStatus ConfirmedBySender = new(103, "تایید شده توسط ارسال‌کننده", nameof(ConfirmedBySender));
    public static readonly RequestProcessStatus CashPayment = new(104, "پرداخت نقدی", nameof(CashPayment));
    public static readonly RequestProcessStatus IsPaid = new(105, "پرداخت شده", nameof(IsPaid));
    public static readonly RequestProcessStatus PickedUp = new(106, "تحویل گرفته شده", nameof(PickedUp));
    public static readonly RequestProcessStatus PassengerConfirmedDelivery = new(107, "تحویل داده شده", nameof(PassengerConfirmedDelivery));
    public static readonly RequestProcessStatus Finalize = new(108, "تحویل گرفته شده", nameof(Finalize));
    public static readonly RequestProcessStatus NotDelivered = new(109, "تحویل داده نشد", nameof(NotDelivered));

    public string PersianName { get; }

    public string EnglishName { get; }

    private RequestProcessStatus(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}