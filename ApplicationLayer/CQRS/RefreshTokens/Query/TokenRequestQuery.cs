using ApplicationLayer.DTOs;
using MediatR;

namespace ApplicationLayer.CQRS.RefreshTokens.Query;

public record TokenRequestQuery(TokenRequestDto Model) : IRequest<HandlerResult>;