using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class OrderStatusEnum : SmartEnum<OrderStatusEnum>
{
    public static readonly OrderStatusEnum Pending = new(1, "آقا", nameof(Pending));
    public static readonly OrderStatusEnum Paid = new(2, "خانم", nameof(Paid));
    public static readonly OrderStatusEnum Failed = new(3, "خانم", nameof(Failed));
    public static readonly OrderStatusEnum Canceled = new(4, "خانم", nameof(Canceled));

    public string PersianName { get; }

    public string EnglishName { get; }

    private OrderStatusEnum(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}