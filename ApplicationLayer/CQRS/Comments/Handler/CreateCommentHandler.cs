using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Comments.Command;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.Comments.Handler;

public class CreateCommentHandler(IUnitOfWork unitOfWork, ICommentServices commentService) : IRequestHandler<CreateCommentCommand, HandlerResult>
{
    private readonly ICommentServices _commentService = commentService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<HandlerResult> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var result = await _commentService.CreateAsync(request.Model, cancellationToken);
        if (result.RequestStatus != RequestStatus.Successful)
            return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };
    }
}