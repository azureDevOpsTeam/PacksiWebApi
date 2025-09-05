using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.DTOs.Dashboard;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using ApplicationLayer.Interfaces;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.BusinessLogic.Services;

[InjectAsScoped]
public class DashboardService(IRepository<Request> requestRepository, IUserContextService userContextService, IRepository<UserProfile> userProfileRepository, IRepository<UserPreferredLocation> userPreferredLocation, ILogger<DashboardService> logger) : IDashboardService
{
    private readonly IRepository<Request> _requestRepository = requestRepository;
    private readonly IUserContextService _userContextService = userContextService;
    private readonly IRepository<UserProfile> _userProfileRepository = userProfileRepository;
    private readonly IRepository<UserPreferredLocation> _userPreferredLocation = userPreferredLocation;
    private readonly ILogger<DashboardService> _logger = logger;

    public async Task<ServiceResult> ReportTripsAsync()
    {
        try
        {
            var currentUserId = _userContextService.UserId;

            var userCountryId = await _userProfileRepository.Query()
                .Where(p => p.UserAccountId == currentUserId)
                .Select(p => p.CountryOfResidenceId)
                .FirstOrDefaultAsync();

            var preferredCountryIds = await _userPreferredLocation.Query()
                .Where(p => p.UserAccountId == currentUserId && p.CountryId != null)
                .Select(p => p.CountryId.Value)
                .ToListAsync();

            var relatedRequests = await _requestRepository.Query()
                .Where(r =>
                    r.OriginCity != null &&
                    r.DestinationCity != null &&
                    (r.OriginCity.CountryId == userCountryId || preferredCountryIds.Contains(r.OriginCity.CountryId)) &&
                    (r.DestinationCity.CountryId == userCountryId || preferredCountryIds.Contains(r.DestinationCity.CountryId)))
                .Select(r => new
                {
                    r.RequestType,
                    OriginCountryId = r.OriginCity.CountryId,
                    DestinationCountryId = r.DestinationCity.CountryId
                })
                .ToListAsync();

            var reportTripsDto = new ReportTripsDto
            {
                CarryerOutboundTrips = relatedRequests.Count(r =>
                    r.RequestType == (int)RequestTypeEnum.Passenger &&
                    r.OriginCountryId == userCountryId &&
                    preferredCountryIds.Contains(r.DestinationCountryId)),

                CarryerInboundTrips = relatedRequests.Count(r =>
                    r.RequestType == (int)RequestTypeEnum.Passenger &&
                    preferredCountryIds.Contains(r.OriginCountryId) &&
                    r.DestinationCountryId == userCountryId),

                SenderOutboundTrips = relatedRequests.Count(r =>
                    r.RequestType == (int)RequestTypeEnum.Sender &&
                    r.OriginCountryId == userCountryId &&
                    preferredCountryIds.Contains(r.DestinationCountryId)),

                SenderInboundTrips = relatedRequests.Count(r =>
                    r.RequestType == (int)RequestTypeEnum.Sender &&
                    preferredCountryIds.Contains(r.OriginCountryId) &&
                    r.DestinationCountryId == userCountryId)
            };

            return new ServiceResult().Successful(reportTripsDto);
        }
        catch (Exception ex)
        {
            return new ServiceResult().Failed(_logger, ex, CommonExceptionMessage.GetFailed("آمار پروازها"));
        }
    }

    public async Task<ServiceResult> InboundOutboundTripsAsync()
    {
        try
        {
            var currentUserId = _userContextService.UserId;

            var userCountryId = await _userProfileRepository.Query()
               .Where(p => p.UserAccountId == currentUserId)
               .Select(p => p.CountryOfResidenceId)
               .FirstOrDefaultAsync();

            var preferredCountryIds = await _userPreferredLocation.Query()
                .Where(p => p.UserAccountId == currentUserId && p.CountryId != null)
                .Select(p => p.CountryId.Value)
                .ToListAsync();

            var requests = await _requestRepository.Query()
                .Where(r =>
                    r.OriginCity != null && r.DestinationCity != null &&
                    (
                        (r.OriginCity.CountryId == userCountryId && preferredCountryIds.Contains(r.DestinationCity.CountryId)) ||
                        (preferredCountryIds.Contains(r.OriginCity.CountryId) && r.DestinationCity.CountryId == userCountryId)
                    )
                )
                .Include(r => r.OriginCity)
                .Include(r => r.DestinationCity)
                .Include(r => r.RequestItemTypes)
                .ToListAsync();

            List<TripDto> SelectTop3(List<Request> source, int requestType)
            {
                return source
                    .Where(r => r.RequestType == requestType)
                    .OrderByDescending(r => r.DepartureDate)
                    .Take(3)
                    .Select(r => new TripDto
                    {
                        DepartureDate = r.DepartureDate,
                        ItemTypes = r.RequestItemTypes
                            .Select(t => TransportableItemTypeEnum.FromValue(t.ItemType).PersianName)
                            .ToList()
                    })
                    .ToList();
            }

            var outbound = requests
                .Where(r => r.OriginCity.CountryId == userCountryId && preferredCountryIds.Contains(r.DestinationCity.CountryId))
                .ToList();

            var inbound = requests
                .Where(r => preferredCountryIds.Contains(r.OriginCity.CountryId) && r.DestinationCity.CountryId == userCountryId)
                .ToList();

            var result = new InboundOutboundTripsDto
            {
                OutboundTrips = SelectTop3(outbound, (int)RequestTypeEnum.Passenger)
                    .Concat(SelectTop3(outbound, (int)RequestTypeEnum.Sender))
                    .ToList(),

                InboundTrips = SelectTop3(inbound, (int)RequestTypeEnum.Passenger)
                    .Concat(SelectTop3(inbound, (int)RequestTypeEnum.Sender))
                    .ToList()
            };

            return new ServiceResult().Successful(result);
        }
        catch (Exception ex)
        {
            return new ServiceResult().Failed(_logger, ex, CommonExceptionMessage.GetFailed("لیست پروازهای ورودی/خروجی داشبورد"));
        }
    }
}