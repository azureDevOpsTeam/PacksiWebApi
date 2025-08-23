using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.CQRS.MiniApp.Handler;

public class VerifyPhoneNumberTMAHandler(IUnitOfWork unitOfWork, IMiniAppServices miniAppServices, IConfiguration configuration, IUserAccountServices userAccountServices, ILogger<VerifyPhoneNumberTMAHandler> logger, IMapper mapper) : IRequestHandler<VerifyPhoneNumberTMACommand, HandlerResult>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<VerifyPhoneNumberTMAHandler> _logger = logger;
    private readonly IMapper _mapper = mapper;
    public async Task<HandlerResult> Handle(VerifyPhoneNumberTMACommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _userAccountServices.MiniApp_VerifyPhoneNumberAsync(request.Model);
            if (result.RequestStatus != RequestStatus.Successful)
                return new HandlerResult { RequestStatus = result.RequestStatus, Message = result.Message };

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new HandlerResult { RequestStatus = RequestStatus.Successful, Message = CommonMessages.Successful };
        }
        catch (Exception)
        {

            throw;
        }
    }
}
