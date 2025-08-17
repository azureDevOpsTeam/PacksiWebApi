using ApplicationLayer.DTOs.RefreshTokens;
using MediatR;

namespace ApplicationLayer.CQRS.RefreshTokens.Command;

public record RevokeRefreshTokenCommand(RevokeRefreshTokenDto Model) : IRequest<HandlerResult>;