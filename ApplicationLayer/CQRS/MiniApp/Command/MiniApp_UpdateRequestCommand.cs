using MediatR;
using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record MiniApp_UpdateRequestCommand(
    int RequestId,
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
    bool IsDraft,
    List<int> ItemTypeIds,
    List<IFormFile> Files
) : IRequest<HandlerResult>;