using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.MiniApp;
using ApplicationLayer.DTOs.Requests;
using ApplicationLayer.DTOs.TelegramApis;
using ApplicationLayer.Extensions.SmartEnums;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface IMiniAppServices
{
    Task<Result<TelegramInfoDto>> CheckUserExistenceAsync(long telegramUserId);

    Task<Result<TelegramMiniAppUserDto>> ExtractUserInfoFromInitDataAsync(string initData);

    Task<Result<TelegramMiniAppValidationResultDto>> ValidateTelegramMiniAppUserAsync();

    Task<Result<bool>> SendMessageAsync(long chatId);

    Task<Result<List<RequestItemTypeDto>>> ItemTypeAsync();

    Task<Result<List<TripsDto>>> GetRequestTripsAsync(UserAccount user);

    Task<Result<List<UserRequestsDto>>> UserRequestsAsync(UserAccount user);

    Task<Result<List<MyPostedSelectedDto>>> MyPostedSelectedAsync(UserAccount user);

    Task<Result<List<UserRequestsDto>>> MySelectionsAsync(UserAccount user);

    Task<Result<RequestDetailDto>> GetRequestByIdAsync(RequestKeyDto model);

    Task<Result<Request>> AddRequestAsync(MiniApp_CreateRequestCommand model, UserAccount userAccount, CancellationToken cancellationToken);

    Task<Result> AddRequestItemTypeAsync(MiniApp_CreateRequestCommand model, int requestId);

    Task<Result> ConfirmedBySenderAsync(RequestSuggestionKeyDto model, UserAccount userAccount);

    Task<Result> RejectByManagerAsync(RequestKeyDto model);

    Task<Result> PublishedRequestAsync(RequestKeyDto model);

    Task<Result> ReadyToPickupAsync(RequestSuggestionKeyDto model);

    Task<Result> PickedUpAsync(RequestSuggestionKeyDto model);

    Task<Result> InTransitAsync(RequestSuggestionKeyDto model);

    Task<Result> ReadyToDeliverAsync(RequestSuggestionKeyDto model);

    Task<Result> SaveRatingAsync(AddRatingDto model, UserAccount user);

    Task<Result> NotDeliveredAsync(RequestSuggestionKeyDto model);

    Task<Result> RejectSelectionAsync(RequestSuggestionKeyDto model);

    Task<Result> AddHistoryStatusAsync(Suggestion suggestion, RequestProcessStatus processStatus, UserAccount user);

    Task<Result<List<TripsDto>>> GetMyRequestsAsync(UserAccount user);

    Task<Result<Suggestion>> CreateSuggestionAsync(MiniApp_CreateSuggestionCommand model, UserAccount user);

    Task<Result<RequestSuggestionDto>> GetSuggestionAsync(RequestSuggestionKeyDto model);

    Task<Result<List<SuggestionAttachment>>> CreateSuggestionAttachmentAsync(List<IFormFile> files, int suggestionId);

    Task<Result> PassengerConfirmedDeliveryAsync(RequestSuggestionKeyDto model, UserAccount user);

    Task<Result<RequestInprogressDto>> GetInProgressRequestAsync(UserAccount user);

    Task<Result<List<RequestAttachment>>> AddRequestAttachmentAsync(int requestId, List<IFormFile> files, RequestTypeEnum requestType);
    Task<Result<List<UserAccountDto>>> GetMyInvitedUsersAsync(long telegramId);
}