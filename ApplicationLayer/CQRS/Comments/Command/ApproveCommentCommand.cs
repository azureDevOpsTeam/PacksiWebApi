using ApplicationLayer.DTOs.BaseDTOs;
using MediatR;

namespace ApplicationLayer.CQRS.Comments.Command;

public record ApproveCommentCommand(KeyDto Model) : IRequest<HandlerResult>;