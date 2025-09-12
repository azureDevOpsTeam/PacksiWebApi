using MediatR;
using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record MiniApp_CreateSuggestionCommand(
     int RequestId,
     decimal SuggestionPrice,
     int Currency,
     string Description,
     int ItemTypeId,
     List<IFormFile> Files
) : IRequest<HandlerResult>;