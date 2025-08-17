using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.DTOs.BaseDTOs;
using ApplicationLayer.DTOs.Comments;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.BusinessLogic.Services;

[InjectAsScoped]
internal class CommentServices(IRepository<Comment> commentRepository, ILogger<CommentServices> logger) : ICommentServices
{
    private readonly IRepository<Comment> _commentRepository = commentRepository;

    private readonly ILogger<CommentServices> _logger = logger;

    public async Task<ServiceResult> CreateAsync(CreateCommentDto model, CancellationToken cancellationToken)
    {
        try
        {
            var comment = new Comment
            {
                Content = model.Content,
                IsApproved = false
            };

            await _commentRepository.AddAsync(comment);
            return new ServiceResult()
            {
                RequestStatus = RequestStatus.Successful,
                Message = CommonMessages.Successful,
            };
        }
        catch (Exception excepotion)
        {
            return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("ثبت نظر"));
        }
    }

    public async Task<ServiceResult> ApproveAsync(KeyDto model, CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _commentRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken);

            if (comment == null)
                return new ServiceResult { RequestStatus = RequestStatus.NotFound, Message = CommonMessages.NotFound };

            comment.IsApproved = true;
            return new ServiceResult()
            {
                RequestStatus = RequestStatus.Successful,
                Message = CommonMessages.Successful,
            };
        }
        catch (Exception excepotion)
        {
            return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("تایید نظر"));
        }
    }

    public async Task<ServiceResult> GetPendingCommentsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _commentRepository.Query()
                .Where(current => !current.IsApproved)
                .Include(current => current.UserAccount)
                    .ThenInclude(current => current.UserProfiles)
                .OrderByDescending(current => current.Id)
                .Select(current => new PendingCommentDto
                {
                    Id = current.Id,
                    Content = current.Content,
                    DisplayName = current.UserAccount.UserProfiles.FirstOrDefault().DisplayName,
                    PhoneNumber = $"{current.UserAccount.PhonePrefix}{current.UserAccount.PhoneNumber}"
                })
                .ToListAsync(cancellationToken);

            if (!response.Any()) return new ServiceResult { RequestStatus = RequestStatus.NotFound, Message = CommonMessages.NotFound };

            return new ServiceResult().Successful(response);
        }
        catch (Exception excepotion)
        {
            return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("نظات تایید نشده"));
        }
    }

    public async Task<ServiceResult> GetCommentsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _commentRepository.Query()
                .Where(current => current.IsApproved)
                .Include(current => current.UserAccount)
                    .ThenInclude(current => current.UserProfiles)
                .OrderByDescending(current => current.Id)
                .Select(current => new PendingCommentDto
                {
                    Id = current.Id,
                    Content = current.Content,
                    DisplayName = current.UserAccount.UserProfiles.FirstOrDefault().DisplayName,
                    PhoneNumber = $"{current.UserAccount.PhonePrefix}{current.UserAccount.PhoneNumber}"
                })
                .Take(10)
                .ToListAsync(cancellationToken);

            if (!response.Any()) return new ServiceResult { RequestStatus = RequestStatus.NotFound, Message = CommonMessages.NotFound };

            return new ServiceResult().Successful(response);
        }
        catch (Exception excepotion)
        {
            return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("نظات تایید نشده"));
        }
    }

    public async Task<ServiceResult> GetByIdAsync(KeyDto model, CancellationToken cancellationToken)
    {
        try
        {
            var comment = await _commentRepository.Query()
            .Include(c => c.UserAccount)
               .ThenInclude(current => current.UserProfiles)
            .FirstOrDefaultAsync(c => c.Id == model.Id, cancellationToken);

            if (comment == null)
                return new ServiceResult { RequestStatus = RequestStatus.NotFound, Message = CommonMessages.NotFound };

            var response = new CommentDetailDto
            {
                Id = comment.Id,
                Content = comment.Content,
                IsApproved = comment.IsApproved,
                UserAccountId = comment.UserAccountId,
                DisplayName = comment.UserAccount.UserProfiles.FirstOrDefault().DisplayName,
                PhoneNumber = $"{comment.UserAccount.PhonePrefix}{comment.UserAccount.PhoneNumber}"
            };
            return new ServiceResult().Successful(response);
        }
        catch (Exception excepotion)
        {
            return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.AddFailed("جزئیات نظر"));
        }
    }
}