using ApplicationLayer.DTOs.User;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record AddUserPreferredLocationWithStartCommand(CountryOfResidenceDto Model) : IRequest<HandlerResult>;