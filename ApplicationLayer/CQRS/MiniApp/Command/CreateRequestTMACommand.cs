using MediatR;
using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.CQRS.MiniApp.Command;

//public record CreateRequestTMACommand(CreateRequestTMADto Model) : IRequest<HandlerResult>;
public record MiniApp_CreateRequestCommand(
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
