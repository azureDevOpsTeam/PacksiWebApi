using ApplicationLayer.DTOs.Comments;
using MediatR;

namespace ApplicationLayer.CQRS.Comments.Command;
public record CreateCommentCommand(CreateCommentDto Model) : IRequest<HandlerResult>;