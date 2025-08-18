using ApplicationLayer.DTOs.MiniApp;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Query;
public record UserValicationQuery(UserValidationDto Model) : IRequest<HandlerResult>;