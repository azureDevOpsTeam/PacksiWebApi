using MediatR;
using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.CQRS.Requests.Command;

//public record CreateRequestCommand(CreateRequestDto Model) : IRequest<HandlerResult>;
public record CreateRequestCommand(
    int OriginCityId,
    int DestinationCityId,
    DateTime DepartureDate,
    DateTime ArrivalDate,
    int RequestType,
    string Description,
    double? MaxWeightKg,
    double? MaxLengthCm,
    double? MaxWidthCm,
    double? MaxHeightCm,
    List<int> ItemTypeIds,
    List<IFormFile> Files
) : IRequest<HandlerResult>;