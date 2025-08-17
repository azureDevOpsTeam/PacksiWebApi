using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.Managers.Command;
using ApplicationLayer.Extensions.SmartEnums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.Managers.Handler;

public class InviteUserHandler(IUnitOfWork unitOfWork, ILogger<InviteUserHandler> logger, IManagerService managerService) : IRequestHandler<InviteUserCommand, HandlerResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<InviteUserHandler> _logger = logger;

    public async Task<HandlerResult> Handle(InviteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await managerService.CreateInviteCodeAsync(request.Model.MaxUsageCount, request.Model.ExpireDate);
            if (result.RequestStatus == RequestStatus.Successful)
                await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message, ObjectResult = result.Data };
        }
        catch (Exception exception)
        {
            return new HandlerResult().Failed(_logger, exception, "InviteUserHandler");
        }
    }
}