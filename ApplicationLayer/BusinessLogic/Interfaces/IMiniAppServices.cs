using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.CQRS.Requests.Command;
using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.MiniApp;
using ApplicationLayer.DTOs.Requests;
using ApplicationLayer.DTOs.TelegramApis;
using ApplicationLayer.Extensions.SmartEnums;
using DomainLayer.Entities;

namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface IMiniAppServices
{
    Task<Result<TelegramInfoDto>> CheckUserExistenceAsync(long telegramUserId);

    Task<Result<TelegramMiniAppUserDto>> ExtractUserInfoFromInitDataAsync(string initData);

    Task<Result<TelegramMiniAppValidationResultDto>> ValidateTelegramMiniAppUserAsync();

    Task<Result<bool>> SendMessageAsync(long chatId);

    Task<Result<List<RequestItemTypeDto>>> ItemTypeAsync();

    Task<Result<List<TripsDto>>> GetRequestsAsync(UserAccount user);

    Task<Result<List<TripsDto>>> InboundTripsQueryAsync(UserAccount user);

    //Task<Result<List<TripsDto>>> OutboundTripsAsync(UserAccount user);

    Task<Result<List<UserRequestsDto>>> UserRequestsAsync(UserAccount user);

    Task<Result<List<MyPostedSelectedDto>>> MyPostedSelectedAsync(UserAccount user);

    Task<Result<List<UserRequestsDto>>> MySelectionsAsync(UserAccount user);

    Task<Result<RequestDetailDto>> GetRequestByIdAsync(RequestKeyDto model);

    Task<ServiceResult> AddRequestAsync(MiniApp_CreateRequestCommand model, UserAccount userAccount, CancellationToken cancellationToken);

    Task<ServiceResult> AddRequestSelectionAsync(int requestId, UserAccount userAccount, CancellationToken cancellationToken);

    Task<ServiceResult> AddRequestItemTypeAsync(MiniApp_CreateRequestCommand model, int requestId);

    Task<Result<RequestSelection>> SelectedRequestAsync(RequestKeyDto model, UserAccount user);

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

    Task<Result> AddHistoryStatusAsync(RequestSelection requestSelection, RequestProcessStatus processStatus, UserAccount user);
}