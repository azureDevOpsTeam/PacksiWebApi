using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.TelegramApis.Command;
using ApplicationLayer.Extensions.SmartEnums;
using AutoMapper;
using DomainLayer.Entities;
using MediatR;

namespace ApplicationLayer.CQRS.TelegramApis.Handler
{
    public class VerifyTelegramHandler(ITelegramServices telegramServices, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<VerifyTelegramCommand, HandlerResult>
    {
        private readonly ITelegramServices _telegramServices = telegramServices;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<HandlerResult> Handle(VerifyTelegramCommand request, CancellationToken cancellationToken)
        {
            var telegramAccount = _mapper.Map<TelegramUserInformation>(request.Model);

            var result = await _telegramServices.VerifyTelegramAsync(telegramAccount, request.Model.PhoneNumber);
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
}