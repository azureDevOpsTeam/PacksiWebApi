using ApplicationLayer.DTOs.Identity;
using MediatR;

namespace ApplicationLayer.CQRS.Identities.Query;

public record SignInQuery(SignInDto Model) : IRequest<HandlerResult>;