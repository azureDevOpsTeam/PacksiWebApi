using ApplicationLayer.DTOs.BaseDTOs;
using ApplicationLayer.DTOs.Comments;

namespace ApplicationLayer.BusinessLogic.Interfaces;

public interface ICommentServices
{
    Task<ServiceResult> CreateAsync(CreateCommentDto model, CancellationToken cancellationToken);

    Task<ServiceResult> ApproveAsync(KeyDto model, CancellationToken cancellationToken);

    Task<ServiceResult> GetPendingCommentsAsync(CancellationToken cancellationToken);

    Task<ServiceResult> GetByIdAsync(KeyDto model, CancellationToken cancellationToken);

    Task<ServiceResult> GetCommentsAsync(CancellationToken cancellationToken);
}