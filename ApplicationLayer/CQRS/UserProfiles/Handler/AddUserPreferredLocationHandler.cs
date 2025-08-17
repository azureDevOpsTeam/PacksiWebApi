using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.UserProfiles.Command;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;

namespace ApplicationLayer.CQRS.UserProfiles.Handler;

public class AddUserPreferredLocationHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<AddUserPreferredLocationCommand, HandlerResult>
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<HandlerResult> Handle(AddUserPreferredLocationCommand request, CancellationToken cancellationToken)
    {
        var result = await _currentUserService.AddUserPreferredLocation(request.Model);
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