using ApplicationLayer.Extensions.SmartEnums;

namespace ApplicationLayer.DTOs.MiniApp;

public class TripsDto
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

    public int? LastStatus { get; set; }

    public string CurrentUserStatusEn
    {
        get
        {
            if (LastStatus < 100)
                return RequestLifecycleStatus.FromValue(LastStatus.Value).EnglishName;
            else
                return RequestProcessStatus.FromValue(LastStatus.Value).EnglishName;
        }
    }

    public string CurrentUserStatusFa
    {
        get
        {
            if (LastStatus < 100)
                return RequestLifecycleStatus.FromValue(LastStatus.Value).PersianName;
            else
                return RequestProcessStatus.FromValue(LastStatus.Value).PersianName;
        }
    }

    public string TripType { get; set; }

    public string SelectStatus { get; set; }

    public string Pickedme_OperationButton
    {
        get
        {
            if (SelectStatus == "pickedme")
                return LastStatus == RequestProcessStatus.Selected.Value ? "btnSuggtion"
                    : LastStatus == RequestProcessStatus.ConfirmedBySender.Value ? "btnPickedUp" //With Chat
                    : LastStatus == RequestProcessStatus.PickedUp.Value ? "btnPassengerConfirmedDelivery" //With NotDelivered
                    : LastStatus == RequestProcessStatus.PassengerConfirmedDelivery.Value ? "lblWaitToConfirmDelivery"
                    : "btnDisable";
            else
            {
                return "btnDisable";
            }
        }
    }

    public string Ipicked_OperationButton
    {
        get
        {
            if (SelectStatus == "ipicked")
            {
                return LastStatus == RequestProcessStatus.ConfirmedBySender.Value ? "lblReadyToPickeUp" //With Chat
                    : LastStatus == RequestProcessStatus.PickedUp.Value ? "lblReadyToDelivery" //With Chat
                    : LastStatus == RequestProcessStatus.PassengerConfirmedDelivery.Value ? "btnConfirmDelivery"//With NotDelivered
                    : "btnDisable";
            }
            else
            {
                return "btnDisable";
            }
        }
    }

    public List<SuggestionDto> Suggestions { get; set; }

    public List<string> AvailableActions { get; set; } = new();
}

public class SuggestionDto
{
    public int SuggestionId { get; set; }

    public int UserAccountId { get; set; }

    public string FullName { get; set; }

    public decimal Price { get; set; }

    public int Currency { get; set; }

    public string Description { get; set; }

    public int? SuggestionStatus { get; set; }

    public string LastStatusEn
    {
        get
        {
            if (SuggestionStatus.HasValue)
                if (SuggestionStatus > 100)
                    return RequestProcessStatus.FromValue(SuggestionStatus.Value).EnglishName;
            return string.Empty;
        }
    }

    public string LastStatusFa
    {
        get
        {
            if (SuggestionStatus.HasValue)
                if (SuggestionStatus > 100)
                    return RequestProcessStatus.FromValue(SuggestionStatus.Value).PersianName;
            return string.Empty;
        }
    }
}