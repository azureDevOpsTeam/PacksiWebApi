using ApplicationLayer.Extensions.SmartEnums;
using System.Text.Json.Serialization;

namespace ApplicationLayer.DTOs.MiniApp;

public class RequestInprogressDto
{
    public List<RequestInfoDto> MyReciveOffers { get; set; }

    public List<RequestInfoDto> MySentOffers { get; set; }
}

public class RequestInfoDto
{
    public int Id { get; set; }

    public string OriginCityName { get; set; }

    public string OriginCityPersianName { get; set; }

    public string DestinationCityName { get; set; }

    public string DestinationCityPersianName { get; set; }

    public int Status { get; set; }

    public List<ActiveSuggestionDto> Suggestions { get; set; } = new();
}

public class ActiveSuggestionDto
{
    public int Id { get; set; }

    public string DisplayName { get; set; }

    public decimal SuggestionPrice { get; set; }

    public int Currency { get; set; }

    public string Descriptions { get; set; }

    public int ItemType { get; set; }

    public int Status { get; set; }

    [JsonIgnore]
    public OfferContext Context { get; set; }  // پر میشه در کوئری

    public string OperationButton => GetOperationButton(Context, Status);

    private string GetOperationButton(OfferContext context, int status) => (context, status) switch
    {
        (OfferContext.Received, var s) when s == RequestProcessStatus.Selected.Value => "btnSuggtion",
        (OfferContext.Received, var s) when s == RequestProcessStatus.ConfirmedBySender.Value => "btnPickedUp",
        (OfferContext.Received, var s) when s == RequestProcessStatus.PickedUp.Value => "btnPassengerConfirmedDelivery",
        (OfferContext.Received, var s) when s == RequestProcessStatus.PassengerConfirmedDelivery.Value => "lblWaitToConfirmDelivery",

        (OfferContext.Sent, var s) when s == RequestProcessStatus.Selected.Value => "lblWaitForAcceptSuggetion",
        (OfferContext.Sent, var s) when s == RequestProcessStatus.ConfirmedBySender.Value => "lblReadyToPickeUp",
        (OfferContext.Sent, var s) when s == RequestProcessStatus.PickedUp.Value => "lblReadyToDelivery",
        (OfferContext.Sent, var s) when s == RequestProcessStatus.PassengerConfirmedDelivery.Value => "btnConfirmDelivery",

        _ => "btnDisable"
    };

    public DateTime CreatedOn { get; set; }

    public List<string> Attachments { get; set; } = new();
}