using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Comments.Query;
using MediatR;

namespace ApplicationLayer.CQRS.Comments.Handler;

public class GetPendingCommentsHandler(ICommentServices commentService) : IRequestHandler<GetPendingCommentsQuery, HandlerResult>
{
    private readonly ICommentServices _commentService = commentService;

    public async Task<HandlerResult> Handle(GetPendingCommentsQuery request, CancellationToken cancellationToken)
    {
        var result = await _commentService.GetPendingCommentsAsync(cancellationToken);
        return new HandlerResult { RequestStatus = result.RequestStatus, ObjectResult = result.Data, Message = result.Message };
    }
}