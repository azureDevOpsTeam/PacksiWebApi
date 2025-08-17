using Ardalis.SmartEnum;

namespace ApplicationLayer.Extensions.SmartEnums;

public class RequestTypeEnum : SmartEnum<RequestTypeEnum>
{
    public static readonly RequestTypeEnum Carryer = new(1, "حمل کننده", nameof(Carryer));
    public static readonly RequestTypeEnum Sender = new(2, "ارسال کننده", nameof(Sender));

    public string PersianName { get; }

    public string EnglishName { get; }

    private RequestTypeEnum(int id, string persianName, string englishName) : base(englishName, id)
    {
        PersianName = persianName;
        EnglishName = englishName;
    }
}