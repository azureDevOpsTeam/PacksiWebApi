using ApplicationLayer.DTOs.MiniApp;
using ApplicationLayer.DTOs.Requests;
using DomainLayer.Entities;

namespace ApplicationLayer.BusinessLogic.Interfaces
{
    public interface IRequestServices
    {
        Task<ServiceResult> AddRequestAsync(CreateRequestDto dto, CancellationToken cancellationToken);

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

        Task<ServiceResult> MiniApp_AddRequestAsync(CreateRequestTMADto model, UserAccount userAccount, CancellationToken cancellationToken);

        Task<ServiceResult> MiniApp_AddRequestSelectionAsync(int requestId, UserAccount userAccount, CancellationToken cancellationToken);

        #endregion Telegram MiniApp
    }
}