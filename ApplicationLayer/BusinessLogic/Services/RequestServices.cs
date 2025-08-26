using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.CQRS.MiniApp.Command;
using ApplicationLayer.CQRS.Requests.Command;
using ApplicationLayer.DTOs.MiniApp;
using ApplicationLayer.DTOs.Requests;
using ApplicationLayer.Extensions.ServiceMessages;
using ApplicationLayer.Extensions.SmartEnums;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace ApplicationLayer.BusinessLogic.Services
{
    [InjectAsScoped]
    public class RequestServices(IRepository<Request> requestRepository, IRepository<RequestSelection> requestSelection, IRepository<RequestItemType> itemTypeRepo,
        IRepository<RequestAttachment> attachmentRepo, IRepository<UserPreferredLocation> userPreferredLocation,
        IRepository<RequestItemType> requestItemTypeRepository, IRepository<RequestAvailableOrigin> requestAvailableOriginRepository, IRepository<RequestAvailableDestination> requestAvailableDestinationRepository,
        IRepository<UserProfile> userProfileRepository, IMapper mapper,
        ILogger<ManagerService> logger, IUserContextService userContextService) : IRequestServices
    {
        private readonly IRepository<UserProfile> _userProfileRepository = userProfileRepository;
        private readonly IRepository<UserPreferredLocation> _userPreferredLocation = userPreferredLocation;
        private readonly IRepository<Request> _requestRepository = requestRepository;
        private readonly IRepository<RequestSelection> _requestSelection = requestSelection;
        private readonly IRepository<RequestItemType> _itemTypeRepo = itemTypeRepo;
        private readonly IRepository<RequestAttachment> _attachmentRepo = attachmentRepo;
        private readonly IUserContextService _userContextService = userContextService;
        private readonly ILogger<ManagerService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IRepository<RequestItemType> _requestItemTypeRepository = requestItemTypeRepository;
        private readonly IRepository<RequestAvailableOrigin> _requestAvailableOriginRepository = requestAvailableOriginRepository;
        private readonly IRepository<RequestAvailableDestination> _requestAvailableDestinationRepository = requestAvailableDestinationRepository;

        public async Task<ServiceResult> AddRequestAsync(CreateRequestCommand model, CancellationToken cancellationToken)
        {
            try
            {
                var currentUserId = _userContextService.UserId;

                var request = _mapper.Map<Request>(model);
                request.UserAccountId = currentUserId.Value;

                var attachments = new List<CreateRequestAttachmentDto>();
                var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsRoot);

                foreach (var formFile in model.Files)
                {
                    if (formFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(formFile.FileName);
                        var filePath = Path.Combine(uploadsRoot, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        attachments.Add(new CreateRequestAttachmentDto
                        {
                            FilePath = $"/uploads/{fileName}",
                            FileType = formFile.ContentType,
                            AttachmentType = model.RequestType == RequestTypeEnum.Carryer ? AttachmentTypeEnum.Ticket : AttachmentTypeEnum.ItemImage,
                        });
                    }
                }

                //foreach (var file in dto.Attachments)
                //{
                //    await _attachmentRepo.AddAsync(new RequestAttachment
                //    {
                //        RequestId = request.Id,
                //        FilePath = file.FilePath,
                //        FileType = file.FileType,
                //        AttachmentType = file.AttachmentType
                //    });
                //}

                //request.AvailableOrigins = _mapper.Map<List<RequestAvailableOrigin>>(model.AvailableOrigins);
                //request.AvailableDestinations = _mapper.Map<List<RequestAvailableDestination>>(model.AvailableDestinations);
                await _requestRepository.AddAsync(request);

                return new ServiceResult { RequestStatus = RequestStatus.Successful, Data = request, Message = CommonMessages.Successful };
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("ثبت درخواست"));
            }
        }

        public async Task<ServiceResult> AddRequestItemTypeAsync(CreateRequestCommand model, int requestId)
        {
            try
            {
                foreach (var itemType in model.ItemTypeIds)
                {
                    await _itemTypeRepo.AddAsync(new RequestItemType
                    {
                        RequestId = requestId,
                        ItemType = itemType
                    });
                }
                return new ServiceResult { RequestStatus = RequestStatus.Successful, Message = CommonMessages.Successful };
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("ثبت درخواست"));
            }
        }

        public async Task<ServiceResult> AddRequestSelectionAsync(int requestId, CancellationToken cancellationToken)
        {
            try
            {
                var currentUserId = _userContextService.UserId;

                RequestSelection requestSelection = new()
                {
                    RequestId = requestId,
                    UserAccountId = currentUserId.Value
                };

                await _requestSelection.AddAsync(requestSelection);

                return new ServiceResult { RequestStatus = RequestStatus.Successful, Message = CommonMessages.Successful };
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("ثبت وضعیت درخواست"));
            }
        }

        public async Task<ServiceResult> UpdateRequestAsync(UpdateRequestDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var request = await _requestRepository.GetByIdAsync(dto.RequestId);
                if (request == null)
                {
                    return new ServiceResult
                    {
                        RequestStatus = RequestStatus.NotFound,
                        Message = "درخواست مورد نظر یافت نشد."
                    };
                }

                _mapper.Map(dto, request);

                var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsRoot);

                foreach (var formFile in dto.Files)
                {
                    if (formFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(formFile.FileName);
                        var filePath = Path.Combine(uploadsRoot, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        await _attachmentRepo.AddAsync(new RequestAttachment
                        {
                            RequestId = request.Id,
                            FilePath = $"/uploads/{fileName}",
                            FileType = formFile.ContentType,
                            AttachmentType = dto.RequestType == RequestTypeEnum.Carryer ? AttachmentTypeEnum.Ticket : AttachmentTypeEnum.ItemImage,
                        });
                    }
                }

                //foreach (var file in dto.Attachments)
                //{
                //    await _attachmentRepo.AddAsync(new RequestAttachment
                //    {
                //        RequestId = request.Id,
                //        FilePath = file.FilePath,
                //        FileType = file.FileType,
                //        AttachmentType = file.AttachmentType
                //    });
                //}

                await RequestItemType_DeleteByRequestIdAsync(request.Id);
                foreach (var itemType in dto.ItemTypeIds)
                {
                    await _itemTypeRepo.AddAsync(new RequestItemType
                    {
                        RequestId = request.Id,
                        ItemType = itemType
                    });
                }

                // حذف مبدا و مقصد قبلی و ثبت جدید
                await AvailableOrigin_DeleteByRequestIdAsync(request.Id);
                var origins = _mapper.Map<List<RequestAvailableOrigin>>(dto.AvailableOrigins);
                request.AvailableOrigins = origins;

                await AvailableDestination_DeleteByRequestIdAsync(request.Id);
                var destinations = _mapper.Map<List<RequestAvailableDestination>>(dto.AvailableDestinations);
                request.AvailableDestinations = destinations;

                await _requestRepository.UpdateAsync(request);

                return new ServiceResult().Successful();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ServiceResult> OutboundTripsAsync()
        {
            try
            {
                var currentUserId = _userContextService.UserId;
                var userCountryId = await _userProfileRepository.Query()
                    .Where(p => p.UserAccountId == currentUserId)
                    .Select(p => p.CountryOfResidenceId)
                    .FirstOrDefaultAsync();

                var preferredCountryIds = await _userPreferredLocation.Query()
                    .Where(p => p.UserAccountId == currentUserId && p.CountryId != null)
                    .Select(p => p.CountryId.Value)
                    .ToListAsync();

                var outboundRequests = await _requestRepository.Query()
                    .Where(r =>
                        r.OriginCity != null &&
                        r.OriginCity.CountryId == userCountryId &&
                        r.DestinationCity != null &&
                        preferredCountryIds.Contains(r.DestinationCity.CountryId))
                    .Include(r => r.OriginCity).ThenInclude(c => c.Country)
                    .Include(r => r.DestinationCity).ThenInclude(c => c.Country)
                    .ToListAsync();

                return new ServiceResult().Successful(outboundRequests);
            }
            catch (Exception excepotion)
            {
                return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.GetFailed("پروازهای خروجی"));
            }
        }

        public async Task<ServiceResult> InboundTripsQueryAsync()
        {
            try
            {
                var currentUserId = _userContextService.UserId;

                var userCountryId = await _userProfileRepository.Query()
                   .Where(p => p.UserAccountId == currentUserId)
                   .Select(p => p.CountryOfResidenceId)
                   .FirstOrDefaultAsync();

                var preferredCountryIds = await _userPreferredLocation.Query()
                    .Where(p => p.UserAccountId == currentUserId && p.CountryId != null)
                    .Select(p => p.CountryId.Value)
                    .ToListAsync();

                var inboundRequests = await _requestRepository.Query()
                    .Where(r =>
                        r.OriginCity != null &&
                        preferredCountryIds.Contains(r.OriginCity.CountryId) &&
                        r.DestinationCity != null &&
                        r.DestinationCity.CountryId == userCountryId)
                    .Include(r => r.OriginCity).ThenInclude(c => c.Country)
                    .Include(r => r.DestinationCity).ThenInclude(c => c.Country)
                    .ToListAsync();

                return new ServiceResult().Successful(inboundRequests);
            }
            catch (Exception excepotion)
            {
                return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.GetFailed("پروازهای ورودی"));
            }
        }

        public async Task<ServiceResult> GetRequestByIdAsync(RequestKeyDto model)
        {
            try
            {
                var request = await _requestRepository.Query()
                    .Where(r => r.Id == model.RequestId)
                    .Include(r => r.RequestSelections)
                    .Include(r => r.OriginCity).ThenInclude(c => c.Country)
                    .Include(r => r.DestinationCity).ThenInclude(c => c.Country)
                    .Include(r => r.Attachments)
                    .Include(r => r.AvailableOrigins).ThenInclude(o => o.City).ThenInclude(c => c.Country)
                    .Include(r => r.AvailableDestinations).ThenInclude(d => d.City).ThenInclude(c => c.Country)
                    .FirstOrDefaultAsync();

                if (request == null)
                    return new ServiceResult().NotFound();

                var itemTypes = (await _itemTypeRepo.Query()
                    .Where(it => it.RequestId == request.Id)
                    .Select(it => it.ItemType)
                    .ToListAsync())
                    .ToHashSet();

                var itemList = TransportableItemTypeEnum.List
                    .Where(row => itemTypes.Contains(row.Value))
                    .Select(it => new RequestItemTypeDto
                    {
                        ItemTypeId = it.Value,
                        ItemType = it.PersianName,
                    })
                    .ToList();

                var userProfile = await _userProfileRepository.Query()
                    .Where(p => p.UserAccountId == request.UserAccountId)
                    .FirstOrDefaultAsync();

                var dto = new RequestDetailDto
                {
                    Id = request.Id,
                    UserAccountId = request.UserAccountId,
                    CurrentStatus = RequestStatusEnum.FromValue(request.RequestSelections.OrderByDescending(order => order.Id).FirstOrDefault().Status).PersianName,
                    OriginCityName = request.OriginCity?.Name,
                    OriginCountryName = request.OriginCity?.Country?.Name,
                    DestinationCityName = request.DestinationCity?.Name,
                    DestinationCountryName = request.DestinationCity?.Country?.Name,
                    ArrivalDate = request.ArrivalDate,
                    DepartureDate = request.DepartureDate,
                    ItemTypes = itemList,
                    Attachments = request.Attachments.Select(a => new RequestAttachmentDto
                    {
                        FilePath = a.FilePath,
                        FileType = a.FileType,
                        AttachmentType = a.AttachmentType.ToString()
                    }).ToList(),
                    AvailableOrigins = request.AvailableOrigins.Select(o => new LocationDto
                    {
                        CityName = o.City?.Name,
                        CountryName = o.City?.Country?.Name
                    }).ToList(),
                    AvailableDestinations = request.AvailableDestinations.Select(d => new LocationDto
                    {
                        CityName = d.City?.Name,
                        CountryName = d.City?.Country?.Name
                    }).ToList(),
                    UserDisplayName = userProfile?.DisplayName
                };

                return new ServiceResult().Successful(dto);
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.GetFailed("جزئیات درخواست"));
            }
        }

        #region User Request Data

        public async Task<ServiceResult> UserRequestsAsync()
        {
            try
            {
                var currentUserId = _userContextService.UserId;

                var outboundRequests = await _requestRepository.Query()
                    .Include(r => r.OriginCity)
                    .Where(r => r.UserAccountId == currentUserId)
                    .Select(current => new UserRequestsDto
                    {
                        RequestId = current.Id,
                        ArrivalDate = current.ArrivalDate,
                        DepartureDate = current.DepartureDate,
                        OriginCityName = current.OriginCity.Name,
                        DestinationCityName = current.DestinationCity.Name,
                    })
                    .ToListAsync();

                return new ServiceResult().Successful(outboundRequests);
            }
            catch (Exception excepotion)
            {
                return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.GetFailed("لیست درخواست های کاربر"));
            }
        }

        public async Task<ServiceResult> MyPostedSelectedAsync()
        {
            try
            {
                var currentUserId = _userContextService.UserId;

                var outboundRequests = await _requestRepository.Query()
                    .Where(r => r.UserAccountId == currentUserId)
                    .SelectMany(r => r.RequestSelections
                        .Where(rs => rs.UserAccountId != currentUserId)
                        .GroupBy(rs => rs.UserAccountId)
                        .Select(g => g.OrderByDescending(x => x.Id).FirstOrDefault())
                        .Select(rs => new MyPostedSelectedDto
                        {
                            RequestId = r.Id,
                            RequestSelectionId = rs.Id,
                            SelectorUserAccountId = rs.UserAccountId,
                            SelectorFirstName = rs.UserAccount.UserProfiles.FirstOrDefault().FirstName,
                            SelectorLastName = rs.UserAccount.UserProfiles.FirstOrDefault().LastName,
                            LastStatus = rs.Status,
                            LastStatusStr = RequestStatusEnum.FromValue(rs.Status).PersianName
                        }))
                    .ToListAsync();

                return new ServiceResult().Successful(outboundRequests);
            }
            catch (Exception excepotion)
            {
                return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.GetFailed("لیست درخواست های انتخاب شده"));
            }
        }

        public async Task<ServiceResult> MySelectionsAsync()
        {
            try
            {
                var currentUserId = _userContextService.UserId;

                var outboundRequests = await _requestRepository.Query()
                    .Include(r => r.OriginCity)
                    .Where(r => r.RequestSelections.Any(current => current.UserAccountId == currentUserId))
                    .Select(current => new UserRequestsDto
                    {
                        RequestId = current.Id,
                        ArrivalDate = current.ArrivalDate,
                        DepartureDate = current.DepartureDate,
                        OriginCityName = current.OriginCity.Name,
                        DestinationCityName = current.DestinationCity.Name,
                    })
                    .ToListAsync();

                return new ServiceResult().Successful(outboundRequests);
            }
            catch (Exception excepotion)
            {
                return new ServiceResult().Failed(_logger, excepotion, CommonExceptionMessage.GetFailed("لیست درخواست های کاربر"));
            }
        }

        #endregion User Request Data

        #region Change Status

        public async Task<ServiceResult> SelectedRequestAsync(RequestKeyDto model)
        {
            try
            {
                var currentUserId = _userContextService.UserId;
                var request = await _requestRepository.Query()
                    .Where(r => r.Id == model.RequestId).FirstOrDefaultAsync();

                if (request == null)
                    return new ServiceResult().NotFound();

                RequestSelection requestSelection = new()
                {
                    UserAccountId = currentUserId.Value,
                    RequestId = model.RequestId
                };

                await _requestSelection.AddAsync(requestSelection);
                return new ServiceResult().Successful();
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("آنتخاب درخواست"));
            }
        }

        public async Task<ServiceResult> ConfirmedBySenderAsync(RequestSelectionKeyDto model)
        {
            try
            {
                var request = await _requestSelection.Query()
                    .Where(r => r.Id == model.RequestSelectionId).FirstOrDefaultAsync();

                if (request == null)
                    return new ServiceResult().NotFound();

                request.Status = RequestStatusEnum.ConfirmedBySender;
                await _requestSelection.UpdateAsync(request);

                return new ServiceResult().Successful();
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("قبول درخواست"));
            }
        }

        public async Task<ServiceResult> RejectSelectionAsync(RequestSelectionKeyDto model)
        {
            try
            {
                var request = await _requestSelection.Query()
                    .Where(r => r.Id == model.RequestSelectionId).FirstOrDefaultAsync();

                if (request == null)
                    return new ServiceResult().NotFound();

                request.Status = RequestStatusEnum.ConfirmedBySender;
                await _requestSelection.UpdateAsync(request);

                return new ServiceResult().Successful();
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("رد درخواست"));
            }
        }

        public async Task<ServiceResult> RejectByManagerAsync(RequestKeyDto model)
        {
            try
            {
                var request = await _requestRepository.Query()
                    .Where(r => r.Id == model.RequestId).FirstOrDefaultAsync();

                if (request == null)
                    return new ServiceResult().NotFound();

                var requestRecord = await _requestSelection.Query()
                    .FirstOrDefaultAsync(current => current.RequestId == request.Id);

                requestRecord.Status = RequestStatusEnum.RejectedByManager;
                await _requestSelection.UpdateAsync(requestRecord);

                return new ServiceResult().Successful();
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("رد توسط مدیر"));
            }
        }

        public async Task<ServiceResult> PublishedRequestAsync(RequestKeyDto model)
        {
            try
            {
                var request = await _requestRepository.Query()
                    .Where(r => r.Id == model.RequestId).FirstOrDefaultAsync();

                if (request == null)
                    return new ServiceResult().NotFound();

                var requestRecord = await _requestSelection.Query()
                    .FirstOrDefaultAsync(current => current.RequestId == request.Id);

                requestRecord.Status = RequestStatusEnum.Published;
                await _requestSelection.UpdateAsync(requestRecord);

                return new ServiceResult().Successful();
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("انتشار درخواست"));
            }
        }

        public async Task<ServiceResult> ReadyToPickupAsync(RequestSelectionKeyDto model)
        {
            try
            {
                var request = await _requestSelection.Query()
                    .Where(r => r.Id == model.RequestSelectionId).FirstOrDefaultAsync();

                if (request == null)
                    return new ServiceResult().NotFound();

                request.Status = RequestStatusEnum.ReadyToPickup;
                await _requestSelection.UpdateAsync(request);

                return new ServiceResult().Successful();
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("آماده دریافت بار"));
            }
        }

        public async Task<ServiceResult> PickedUpAsync(RequestSelectionKeyDto model)
        {
            try
            {
                var request = await _requestSelection.Query()
                    .Where(r => r.Id == model.RequestSelectionId).FirstOrDefaultAsync();

                if (request == null)
                    return new ServiceResult().NotFound();

                request.Status = RequestStatusEnum.PickedUp;
                await _requestSelection.UpdateAsync(request);

                return new ServiceResult().Successful();
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("تحویل گرفته شده"));
            }
        }

        public async Task<ServiceResult> InTransitAsync(RequestSelectionKeyDto model)
        {
            try
            {
                var request = await _requestSelection.Query()
                    .Where(r => r.Id == model.RequestSelectionId).FirstOrDefaultAsync();

                if (request == null)
                    return new ServiceResult().NotFound();

                request.Status = RequestStatusEnum.InTransit;
                await _requestSelection.UpdateAsync(request);

                return new ServiceResult().Successful();
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("در حال انتقال"));
            }
        }

        public async Task<ServiceResult> ReadyToDeliverAsync(RequestSelectionKeyDto model)
        {
            try
            {
                var request = await _requestSelection.Query()
                    .Where(r => r.Id == model.RequestSelectionId).FirstOrDefaultAsync();

                if (request == null)
                    return new ServiceResult().NotFound();

                request.Status = RequestStatusEnum.ReadyToDeliver;
                await _requestSelection.UpdateAsync(request);

                return new ServiceResult().Successful();
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("آماده تحویل"));
            }
        }

        public async Task<ServiceResult> DeliveredAsync(RequestKeyDto model)
        {
            try
            {
                var currentUserId = _userContextService.UserId;
                var request = await _requestRepository.Query()
                    .Where(r => r.Id == model.RequestId).FirstOrDefaultAsync();

                if (request == null)
                    return new ServiceResult().NotFound();

                var requestRecord = await _requestSelection.Query()
                    .FirstOrDefaultAsync(current => current.UserAccountId == currentUserId && current.RequestId == request.Id);

                requestRecord.Status = RequestStatusEnum.Delivered;
                await _requestSelection.UpdateAsync(requestRecord);

                return new ServiceResult().Successful();
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("تحویل داده شده"));
            }
        }

        public async Task<ServiceResult> NotDeliveredAsync(RequestSelectionKeyDto model)
        {
            try
            {
                var request = await _requestSelection.Query()
                    .Where(r => r.Id == model.RequestSelectionId).FirstOrDefaultAsync();

                if (request == null)
                    return new ServiceResult().NotFound();

                request.Status = RequestStatusEnum.NotDelivered;
                await _requestSelection.UpdateAsync(request);

                return new ServiceResult().Successful();
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("تحویل داده نشد"));
            }
        }

        #endregion Change Status

        private async Task RequestItemType_DeleteByRequestIdAsync(int requestId)
        {
            var entities = await _requestItemTypeRepository.Query()
                .Where(x => x.RequestId == requestId)
                .ToListAsync();

            _requestItemTypeRepository.RemoveRange(entities);
        }

        private async Task AvailableOrigin_DeleteByRequestIdAsync(int requestId)
        {
            var entities = await _requestAvailableOriginRepository.Query()
                .Where(x => x.RequestId == requestId)
                .ToListAsync();

            _requestAvailableOriginRepository.RemoveRange(entities);
        }

        private async Task AvailableDestination_DeleteByRequestIdAsync(int requestId)
        {
            var entities = await _requestAvailableDestinationRepository.Query()
                .Where(x => x.RequestId == requestId)
                .ToListAsync();

            _requestAvailableDestinationRepository.RemoveRange(entities);
        }

        #region Telegram MiniApp

        public async Task<ServiceResult> MiniApp_AddRequestAsync(CreateRequestTMACommand model, UserAccount userAccount, CancellationToken cancellationToken)
        {
            try
            {
                var request = _mapper.Map<Request>(model);
                request.UserAccountId = userAccount.Id;

                var attachments = new List<CreateRequestAttachmentDto>();
                var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsRoot);

                foreach (var formFile in model.Files)
                {
                    if (formFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(formFile.FileName);
                        var filePath = Path.Combine(uploadsRoot, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }

                        attachments.Add(new CreateRequestAttachmentDto
                        {
                            FilePath = $"/uploads/{fileName}",
                            FileType = formFile.ContentType,
                            AttachmentType = model.RequestType == RequestTypeEnum.Carryer ? AttachmentTypeEnum.Ticket : AttachmentTypeEnum.ItemImage,
                        });
                    }
                }

                foreach (var itemType in model.ItemTypeIds)
                {
                    await _itemTypeRepo.AddAsync(new RequestItemType
                    {
                        RequestId = request.Id,
                        ItemType = itemType
                    });
                }

                //foreach (var file in dto.Attachments)
                //{
                //    await _attachmentRepo.AddAsync(new RequestAttachment
                //    {
                //        RequestId = request.Id,
                //        FilePath = file.FilePath,
                //        FileType = file.FileType,
                //        AttachmentType = file.AttachmentType
                //    });
                //}

                //request.AvailableOrigins = _mapper.Map<List<RequestAvailableOrigin>>(model.AvailableOrigins);
                //request.AvailableDestinations = _mapper.Map<List<RequestAvailableDestination>>(model.AvailableDestinations);
                await _requestRepository.AddAsync(request);

                return new ServiceResult { RequestStatus = RequestStatus.Successful, Message = CommonMessages.Successful };
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("ثبت درخواست"));
            }
        }

        public async Task<ServiceResult> MiniApp_AddRequestSelectionAsync(int requestId, UserAccount userAccount, CancellationToken cancellationToken)
        {
            try
            {
                var currentUserId = _userContextService.UserId;

                RequestSelection requestSelection = new()
                {
                    RequestId = requestId,
                    UserAccountId = currentUserId.Value
                };

                await _requestSelection.AddAsync(requestSelection);

                return new ServiceResult { RequestStatus = RequestStatus.Successful, Message = CommonMessages.Successful };
            }
            catch (Exception exception)
            {
                return new ServiceResult().Failed(_logger, exception, CommonExceptionMessage.AddFailed("ثبت وضعیت درخواست"));
            }
        }

        #endregion Telegram MiniApp
    }
}