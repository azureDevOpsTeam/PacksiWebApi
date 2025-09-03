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

    Task<Result<List<UserRequestsDto>>> UserRequestsAsync(UserAccount user);

    Task<Result<List<MyPostedSelectedDto>>> MyPostedSelectedAsync(UserAccount user);

    Task<Result<List<UserRequestsDto>>> MySelectionsAsync(UserAccount user);

    Task<Result<RequestDetailDto>> GetRequestByIdAsync(RequestKeyDto model);

    Task<Result> SelectedRequestAsync(RequestKeyDto model, UserAccount user);

    Task<Result> ConfirmedBySenderAsync(RequestSelectionKeyDto model);

    Task<Result> RejectByManagerAsync(RequestKeyDto model);

    Task<Result> PublishedRequestAsync(RequestKeyDto model);

    Task<Result> ReadyToPickupAsync(RequestSelectionKeyDto model);

    Task<Result> PickedUpAsync(RequestSelectionKeyDto model);

    Task<Result> InTransitAsync(RequestSelectionKeyDto model);

    Task<Result> ReadyToDeliverAsync(RequestSelectionKeyDto model);

    Task<Result> DeliveredAsync(RequestKeyDto model, UserAccount user);

    Task<Result> NotDeliveredAsync(RequestSelectionKeyDto model);

    Task<Result> RejectSelectionAsync(RequestSelectionKeyDto model);
}