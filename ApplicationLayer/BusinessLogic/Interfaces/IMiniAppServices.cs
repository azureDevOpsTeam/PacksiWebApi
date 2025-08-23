using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.Requests;
using ApplicationLayer.DTOs.TelegramApis;

namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface IMiniAppServices
{
    Task<Result<TelegramInfoDto>> CheckUserExistenceAsync(long telegramUserId);

    Task<Result<TelegramMiniAppUserDto>> ExtractUserInfoFromInitDataAsync(string initData);

    Task<Result<TelegramMiniAppValidationResultDto>> ValidateTelegramMiniAppUserAsync();

    Task<Result<bool>> SendMessageAsync(long chatId);
    Task<Result<List<RequestItemTypeDto>>> ItemTypeAsync();
}