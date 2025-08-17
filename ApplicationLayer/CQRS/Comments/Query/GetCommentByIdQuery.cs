using ApplicationLayer.DTOs.BaseDTOs;
using MediatR;

namespace ApplicationLayer.CQRS.Comments.Query;

public record GetCommentByIdQuery(KeyDto Model) : IRequest<HandlerResult>;