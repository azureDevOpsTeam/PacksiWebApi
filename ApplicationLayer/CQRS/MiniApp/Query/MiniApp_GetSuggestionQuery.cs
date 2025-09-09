using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Query;

public record MiniApp_GetSuggestionQuery(RequestSuggestionKeyDto Model) : IRequest<HandlerResult>;