using ApplicationLayer.DTOs.User;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record AddUserDepartureLocationWithStartCommand(CountryOfResidenceDto Model) : IRequest<HandlerResult>;