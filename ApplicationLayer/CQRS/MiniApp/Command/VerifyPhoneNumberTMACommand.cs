using ApplicationLayer.DTOs.MiniApp;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Command;

public record VerifyPhoneNumberTMACommand(VerifyPhoneNumberDto Model) : IRequest<HandlerResult>;
