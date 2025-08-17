using ApplicationLayer.DTOs.Comments;
using MediatR;

namespace ApplicationLayer.CQRS.Comments.Query;

public record GetCommentsQuery : IRequest<HandlerResult>;