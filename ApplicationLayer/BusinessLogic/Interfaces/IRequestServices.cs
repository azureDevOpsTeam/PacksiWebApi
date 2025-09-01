using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.CQRS.Requests.Command;
using ApplicationLayer.DTOs.MiniApp;
using ApplicationLayer.DTOs.Requests;
using DomainLayer.Entities;

namespace ApplicationLayer.BusinessLogic.Interfaces
{
    public interface IRequestServices
    {
        Task<ServiceResult> AddRequestAsync(CreateRequestCommand dto, CancellationToken cancellationToken);

        Task<ServiceResult> UpdateRequestAsync(UpdateRequestDto dto, CancellationToken cancellationToken);

        Task<ServiceResult> InboundTripsQueryAsync();

        Task<ServiceResult> OutboundTripsAsync();

        Task<ServiceResult> GetRequestByIdAsync(RequestKeyDto model);

        #region Manager Control

        Task<ServiceResult> RejectByManagerAsync(RequestKeyDto model);

        Task<ServiceResult> PublishedRequestAsync(RequestKeyDto model);

        #endregion Manager Control

        #region User Control

        Task<ServiceResult> SelectedRequestAsync(RequestKeyDto model);

        Task<ServiceResult> RejectSelectionAsync(RequestSelectionKeyDto model);

        Task<ServiceResult> ConfirmedBySenderAsync(RequestSelectionKeyDto model);

        Task<ServiceResult> ReadyToPickupAsync(RequestSelectionKeyDto model);

        Task<ServiceResult> InTransitAsync(RequestSelectionKeyDto model);

        Task<ServiceResult> DeliveredAsync(RequestKeyDto model);

        Task<ServiceResult> NotDeliveredAsync(RequestSelectionKeyDto model);

        Task<ServiceResult> PickedUpAsync(RequestSelectionKeyDto model);

        Task<ServiceResult> ReadyToDeliverAsync(RequestSelectionKeyDto model);

        Task<ServiceResult> AddRequestSelectionAsync(int requestId, CancellationToken cancellationToken);

        Task<ServiceResult> MyPostedSelectedAsync();

        Task<ServiceResult> UserRequestsAsync();

        #endregion User Control

        #region Telegram MiniApp

        Task<ServiceResult> MiniApp_AddRequestAsync(MiniApp_CreateRequestCommand model, UserAccount userAccount, CancellationToken cancellationToken);

        Task<ServiceResult> MiniApp_AddRequestSelectionAsync(int requestId, UserAccount userAccount, CancellationToken cancellationToken);

        Task<ServiceResult> AddRequestItemTypeAsync(CreateRequestCommand model, int requestId);
        Task<ServiceResult> MiniApp_AddRequestItemTypeAsync(MiniApp_CreateRequestCommand model, int requestId);

        #endregion Telegram MiniApp
    }
}