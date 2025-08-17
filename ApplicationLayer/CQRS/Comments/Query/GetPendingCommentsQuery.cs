using ApplicationLayer.DTOs.Comments;
using MediatR;

namespace ApplicationLayer.CQRS.Comments.Query;

public record GetPendingCommentsQuery : IRequest<HandlerResult>;