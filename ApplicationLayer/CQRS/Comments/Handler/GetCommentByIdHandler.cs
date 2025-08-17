using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Comments.Query;
using MediatR;

namespace ApplicationLayer.CQRS.Comments.Handler;

internal class GetCommentByIdHandler(ICommentServices commentService) : IRequestHandler<GetCommentByIdQuery, HandlerResult>
{
    private readonly ICommentServices _commentService = commentService;

    public async Task<HandlerResult> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _commentService.GetByIdAsync(request.Model, cancellationToken);
        return new HandlerResult { RequestStatus = result.RequestStatus, ObjectResult = result.Data, Message = result.Message };
    }
}