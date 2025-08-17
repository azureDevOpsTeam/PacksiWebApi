using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.DTOs.Advertisements;
using ApplicationLayer.DTOs.BaseDTOs;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.BusinessLogic.Services;

[InjectAsScoped]
public class AdvertisementService(IRepository<Advertisement> adsRepository, IMapper mapper, ILogger<RefreshTokenService> logger, IUserContextService userContextService) : IAdvertisementService
{
    private readonly IRepository<Advertisement> _adsRepository = adsRepository;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<RefreshTokenService> _logger = logger;

    public async Task<ServiceResult> CreateAsync(CreateAdvertisementDto model)
    {
        try
        {
            var currentUserId = userContextService.UserId.Value;

            var advertisement = _mapper.Map<Advertisement>(model);
            advertisement.UserAccountId = currentUserId;

            await _adsRepository.AddAsync(advertisement);
            return new ServiceResult().Successful();
        }
        catch (Exception excepotion)
        {
            return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("ثبت تبلیغات"));
        }
    }

    public async Task<ServiceResult> GetAllAsync(GetAllFilterDto model)
    {
        try
        {
            var advertisments = _adsRepository.Query();

            if (model.Status > 0)
                advertisments = advertisments.Where(a => a.Status == model.Status);

            advertisments = advertisments
                .Skip(model.Pagination.Skip)
                .Take(model.Pagination.PageSize);

            var advertisements = await advertisments.ToListAsync();

            return new ServiceResult().Successful(advertisments);
        }
        catch (Exception excepotion)
        {
            return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("لیست تبلیغات"));
        }
    }

    public async Task<ServiceResult> GetByIdAsync(KeyDto model)
    {
        try
        {
            var advertisment = await _adsRepository.GetByIdAsync(model.Id);
            if (advertisment is null)
                return new ServiceResult().NotFound();

            return new ServiceResult().Successful(advertisment);
        }
        catch (Exception excepotion)
        {
            return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("دریافت تبلیغ"));
        }
    }

    public async Task<ServiceResult> AcceptAsync(KeyDto model)
    {
        try
        {
            var advertisment = await _adsRepository.GetByIdAsync(model.Id);
            if (advertisment is null)
                return new ServiceResult().NotFound();

            advertisment.Status = AdvertismentStatusEnum.AwaitingPayment;
            return new ServiceResult().Successful();
        }
        catch (Exception excepotion)
        {
            return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("تایید برای پرداخت"));
        }
    }

    public async Task<ServiceResult> PublishAsync(KeyDto model)
    {
        try
        {
            var advertisment = await _adsRepository.GetByIdAsync(model.Id);
            if (advertisment is null)
                return new ServiceResult().NotFound();

            advertisment.Status = AdvertismentStatusEnum.Published;

            //TODO Published Advertisment...
            return new ServiceResult().Successful();
        }
        catch (Exception excepotion)
        {
            return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("انتشار تبلیغ"));
        }
    }

    public async Task<ServiceResult> RejectAsync(KeyDto model)
    {
        try
        {
            var advertisment = await _adsRepository.GetByIdAsync(model.Id);
            if (advertisment is null)
                return new ServiceResult().NotFound();

            advertisment.Status = AdvertismentStatusEnum.Rejected;

            return new ServiceResult().Successful();
        }
        catch (Exception excepotion)
        {
            return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("رد تبلیغ"));
        }
    }

    public async Task<ServiceResult> DeleteAsync(KeyDto model)
    {
        try
        {
            var advertisment = await _adsRepository.GetByIdAsync(model.Id);
            if (advertisment is null)
                return new ServiceResult().NotFound();

            _adsRepository.Remove(advertisment);
            return new ServiceResult().Successful();
        }
        catch (Exception excepotion)
        {
            return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("تایید برای پرداخت"));
        }
    }
}