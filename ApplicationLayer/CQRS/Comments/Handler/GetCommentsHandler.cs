using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Comments.Query;
using MediatR;

namespace ApplicationLayer.CQRS.Comments.Handler;

public class GetCommentsHandler(ICommentServices commentService) : IRequestHandler<GetCommentsQuery, HandlerResult>
{
    private readonly ICommentServices _commentService = commentService;

    public async Task<HandlerResult> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
    {
        var result = await _commentService.GetCommentsAsync(cancellationToken);
        return new HandlerResult { RequestStatus = result.RequestStatus, ObjectResult = result.Data, Message = result.Message };
    }
}