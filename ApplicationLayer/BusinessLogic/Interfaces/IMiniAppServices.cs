using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.MiniApp;
using ApplicationLayer.DTOs.Requests;
using ApplicationLayer.DTOs.TelegramApis;
using DomainLayer.Entities;

namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface IMiniAppServices
{
    Task<Result<TelegramInfoDto>> CheckUserExistenceAsync(long telegramUserId);

    Task<Result<TelegramMiniAppUserDto>> ExtractUserInfoFromInitDataAsync(string initData);

    Task<Result<TelegramMiniAppValidationResultDto>> ValidateTelegramMiniAppUserAsync();

    Task<Result<bool>> SendMessageAsync(long chatId);

    Task<Result<List<RequestItemTypeDto>>> ItemTypeAsync();

    Task<Result<List<OutboundDto>>> InboundTripsQueryAsync(UserAccount user);

    Task<Result<List<OutboundDto>>> OutboundTripsAsync(UserAccount user);
}