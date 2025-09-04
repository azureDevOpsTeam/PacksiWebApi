using ApplicationLayer.Extensions.SmartEnums;

namespace ApplicationLayer.DTOs.MiniApp;

public class OutboundDto
{
    public int RequestId { get; set; }

    public int UserAccountId { get; set; }

    public string FullName { get; set; }

    public string OriginCity { get; set; }

    public string OriginCityFa { get; set; }

    public string DestinationCity { get; set; }

    public string DestinationCityFa { get; set; }

    public DateTime DepartureDate { get; set; }

    public string DepartureDatePersian { get; set; }

    public DateTime ArrivalDate { get; set; }

    public string ArrivalDatePersian { get; set; }

    public decimal? SuggestedPrice { get; set; }

    public string[] ItemTypes { get; set; }

    public string[] ItemTypesFa { get; set; }

    public string Description { get; set; }

    public double? MaxWeightKg { get; set; }

    public double? MaxLengthCm { get; set; }

    public double? MaxWidthCm { get; set; }

    public double? MaxHeightCm { get; set; }

    public int? CurrentUserStatus { get; set; }

    public string CurrentUserStatusEn
    {
        get
        {
            if (CurrentUserStatus < 100)
                return RequestLifecycleStatus.FromValue(CurrentUserStatus.Value).EnglishName;
            else
                return RequestProcessStatus.FromValue(CurrentUserStatus.Value).EnglishName;
        }
    }

    public string CurrentUserStatusFa
    {
        get
        {
            if (CurrentUserStatus < 100)
                return RequestLifecycleStatus.FromValue(CurrentUserStatus.Value).PersianName;
            else
                return RequestProcessStatus.FromValue(CurrentUserStatus.Value).PersianName;
        }
    }
}