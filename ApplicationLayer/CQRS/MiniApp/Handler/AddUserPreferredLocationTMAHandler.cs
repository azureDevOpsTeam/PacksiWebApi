using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.MiniApp.Handler;


public class AddUserPreferredLocationTMAHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<AddUserPreferredLocationTMACommand, HandlerResult>
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<HandlerResult> Handle(AddUserPreferredLocationTMACommand request, CancellationToken cancellationToken)
    {
        var result = await _currentUserService.MiniApp_AddUserPreferredLocationAsync(request.Model);
        if (result.RequestStatus == RequestStatus.Successful)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new HandlerResult()
        {
            RequestStatus = result.RequestStatus,
            ObjectResult = result.Data,
            Message = result.Message
        };
    }
}
