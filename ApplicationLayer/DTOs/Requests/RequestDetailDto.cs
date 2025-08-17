using DNTPersianUtils.Core;
using DomainLayer.Entities;

namespace ApplicationLayer.DTOs.Requests;

public class RequestDetailDto
{
    public int Id { get; set; }

    public int UserAccountId { get; set; }

    public string CurrentStatus { get; set; }

    public string OriginCityName { get; set; }

    public string OriginCountryName { get; set; }

    public string DestinationCityName { get; set; }

    public string DestinationCountryName { get; set; }

    public DateTime DepartureDate { get; set; }

    public DateTime ArrivalDate { get; set; }

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

    public List<RequestItemTypeDto> ItemTypes { get; set; }

    public List<RequestAttachmentDto> Attachments { get; set; }

    public List<LocationDto> AvailableOrigins { get; set; }

    public List<LocationDto> AvailableDestinations { get; set; }

    public string UserDisplayName { get; set; }
}