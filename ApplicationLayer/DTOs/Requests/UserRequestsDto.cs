using DNTPersianUtils.Core;

namespace ApplicationLayer.DTOs.Requests;

public class UserRequestsDto
{
    public int RequestId { get; set; }

    public string OriginCityName { get; set; }

    public string DestinationCityName { get; set; }

    public DateTime DepartureDate { get; set; }

    public DateTime? ArrivalDate { get; set; }

    public string DeparturePersianDate
    {
        get
        {
            return DepartureDate.ToPersianDateTimeString("yyyy/MM/dd");
        }
    }

    public string ArrivalPersianDate
    {
        get
        {
            return ArrivalDate.ToPersianDateTimeString("yyyy/MM/dd");
        }
    }
}