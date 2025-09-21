using ApplicationLayer.DTOs.Requests;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record MiniApp_SaveRatingCommand(AddRatingDto Model) : IRequest<HandlerResult>;