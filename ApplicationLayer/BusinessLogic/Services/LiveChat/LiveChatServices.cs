using ApplicationLayer.BusinessLogic.Interfaces.LiveChat;
using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.LiveChat;
using ApplicationLayer.Extensions;
using ApplicationLayer.Extensions.SmartEnums;
using AutoMapper;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using DomainLayer.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApplicationLayer.BusinessLogic.Services.LiveChat;

[InjectAsScoped]
public class LiveChatServices(IRepository<UserAccount> userAccountRepository, IRepository<Conversation> conversationRepository,
    IRepository<Message> messageRepository, IUnitOfWork unitOfWork, ILogger<LiveChatServices> logger, IMapper mapper) : ILiveChatServices
{
    private readonly IRepository<UserAccount> _userAccountRepository = userAccountRepository;
    private readonly IRepository<Conversation> _conversationRepository = conversationRepository;
    private readonly IRepository<Message> _messageRepository = messageRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<LiveChatServices> _logger = logger;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<List<ChatListDto>>> GetUsersListAsync(UserAccount currentUser)
    {
        try
        {
            var conversations = await _conversationRepository.Query()
                .Where(c => c.User1Id == currentUser.Id || c.User2Id == currentUser.Id)
                .Include(c => c.User1)
                    .ThenInclude(u => u.UserProfiles)
                .Include(c => c.User2)
                    .ThenInclude(u => u.UserProfiles)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1)) // فقط آخرین پیام
                .ToListAsync();

            var chatList = conversations.Select(c =>
            {
                var otherUser = c.User1Id == currentUser.Id ? c.User2 : c.User1;
                var lastMessage = c.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault();

                return new ChatListDto
                {
                    ConversationId = c.Id,
                    SenderId = currentUser.Id,
                    ReciverId = otherUser.Id,
                    RequestCreatorDisplayName = currentUser.Id != c.User1Id ? currentUser.UserProfiles.FirstOrDefault()?.DisplayName : otherUser.UserProfiles.FirstOrDefault()?.DisplayName,
                    Avatar = otherUser.Avatar,
                    IsOnline = true, // اینو باید از Presence یا SignalR دربیاری
                    LastMessage = lastMessage?.Content ?? "No messages yet",
                    LastSeenEn = DateTimeHelper.GetTimeAgo(DateTime.Now.AddMinutes(-28)).En,
                    LastSeenFa = DateTimeHelper.GetTimeAgo(DateTime.Now.AddMinutes(-28)).Fa,
                    IsBlocked = (c.User1Id == currentUser.Id && c.IsUser1Blocked) ||
                                (c.User2Id == currentUser.Id && c.IsUser2Blocked)
                };
            })
            .DistinctBy(c => c.ReciverId)
            .ToList();

            return Result<List<ChatListDto>>.Success(chatList);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت لیست کانورسیشن‌های کاربر {UserId}", currentUser.Id);
            return Result<List<ChatListDto>>.GeneralFailure("خطا در دریافت لیست کاربران");
        }
    }

    //public async Task<Result<List<ChatListDto>>> GetUsersListAsync(UserAccount currentUser)
    //{
    //    try
    //    {
    //        var requestSelections = await _requestSelectionRepository.Query()
    //            .Where(rs => (rs.UserAccountId == currentUser.Id || rs.Request.UserAccountId == currentUser.Id) &&
    //            rs.Status == RequestProcessStatus.ConfirmedBySender)
    //            .Include(rs => rs.Request)
    //                .ThenInclude(rs => rs.UserAccount)
    //                    .ThenInclude(u => u.UserProfiles)
    //            .Include(rs => rs.UserAccount)
    //                .ThenInclude(u => u.UserProfiles)
    //                .ToListAsync();

    //        var requestConversation = await _requestSelectionRepository.Query()
    //            .Where(rs => (rs.UserAccountId == currentUser.Id || rs.Request.UserAccountId == currentUser.Id) &&
    //            rs.Status == RequestProcessStatus.ConfirmedBySender)
    //            .Include(rs => rs.Request)
    //                .ThenInclude(rs => rs.UserAccount)
    //                    .ThenInclude(u => u.UserProfiles)
    //            .Include(rs => rs.UserAccount)
    //                .ThenInclude(u => u.UserProfiles)
    //                .ToListAsync();

    //        var chatList = requestSelections
    //            .Select(rs => new ChatListDto
    //            {
    //                RequestId = rs.Request.Id,
    //                ReciverId = rs.Request.UserAccountId != currentUser.Id ?  rs.Request.UserAccountId : rs.UserAccount.Id,
    //                SenderId = currentUser.Id,
    //                RequestCreatorDisplayName = rs.Request.UserAccountId != currentUser.Id ? rs.Request.UserAccount.UserProfiles.FirstOrDefault()?.DisplayName : rs.UserAccount.UserProfiles.FirstOrDefault()?.DisplayName,
    //                Avatar = rs.Request.UserAccount.Avatar,
    //                IsOnline = true,
    //                LastSeenEn = DateTimeHelper.GetTimeAgo(DateTime.Now.AddMinutes(-28)).En,
    //                LastSeenFa = DateTimeHelper.GetTimeAgo(DateTime.Now.AddMinutes(-28)).Fa,
    //                LastMessage = "click to see message",
    //                IsBlocked = false,
    //            })
    //            .DistinctBy(c => c.ReciverId)
    //            .ToList();

    //        return Result<List<ChatListDto>>.Success(chatList);
    //    }
    //    catch (Exception exception)
    //    {
    //        _logger.LogError(exception, "خطا در دریافت لیست کاربران {UserId}", currentUser.Id);
    //        return Result<List<ChatListDto>>.GeneralFailure("خطا در دریافت لیست کاربران");
    //    }
    //}

    public async Task<Result<MessageDto>> SendMessageAsync(SendMessageDto model, UserAccount sender)
    {
        try
        {
            // Check if receiver exists
            var receiver = await _userAccountRepository.GetByIdAsync(model.ReceiverId);
            if (receiver == null)
                return Result<MessageDto>.NotFound($"کاربر مقصد یافت نشد{model.ReceiverId}");

            // Get or create conversation
            var conversationResult = await GetOrCreateConversationAsync(model.ReceiverId, sender);
            if (!conversationResult.IsSuccess)
                return Result<MessageDto>.GeneralFailure(conversationResult.Message);

            var conversation = conversationResult.Value;

            // Check if sender is blocked
            if ((conversation.User1Id == sender.Id && conversation.IsUser1Blocked) ||
                (conversation.User2Id == sender.Id && conversation.IsUser2Blocked))
            {
                return Result<MessageDto>.GeneralFailure("شما توسط این کاربر بلاک شده‌اید");
            }

            // Create message
            var message = new Message
            {
                ConversationId = conversation.Id,
                SenderId = sender.Id,
                Content = model.Content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _messageRepository.AddAsync(message);

            // Update conversation last message time
            var conversationEntity = await _conversationRepository.GetByIdAsync(conversation.Id);
            if (conversationEntity != null)
            {
                conversationEntity.LastMessageAt = DateTime.UtcNow;
                await _conversationRepository.UpdateAsync(conversationEntity);
            }

            // Add domain event for real-time messaging
            var messageSentEvent = new MessageSentEvent(
                message.Id,
                message.ConversationId,
                sender.Id,
                model.ReceiverId,
                message.Content,
                message.SentAt
            );

            message.AddDomainEvent(messageSentEvent);

            await _unitOfWork.SaveChangesAsync();

            var messageDto = new MessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = sender.UserName ?? sender.TelegramUserName ?? $"User{sender.Id}",
                Content = message.Content,
                SentAt = message.SentAt,
                IsRead = message.IsRead,
                ReadAt = message.ReadAt,
                IsEdited = message.IsEdited,
                EditedAt = message.EditedAt
            };

            return Result<MessageDto>.Success(messageDto);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ارسال پیام {SenderId} به {ReceiverId}", sender.Id, model.ReceiverId);
            return Result<MessageDto>.GeneralFailure("خطا در ارسال پیام");
        }
    }

    public async Task<Result<List<ConversationDto>>> GetRecentConversationsAsync(UserAccount currentUser)
    {
        try
        {
            var conversations = await _conversationRepository.Query()
                .Where(c => c.User1Id == currentUser.Id || c.User2Id == currentUser.Id)
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .OrderByDescending(c => c.LastMessageAt)
                .Select(c => new ConversationDto
                {
                    Id = c.Id,
                    User1Id = c.User1Id,
                    User2Id = c.User2Id,
                    User1Name = c.User1.UserName,
                    User2Name = c.User2.UserName,
                    LastMessageAt = c.LastMessageAt,
                    LastMessageContent = c.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault().Content,
                    IsUser1Blocked = c.IsUser1Blocked,
                    IsUser2Blocked = c.IsUser2Blocked,
                    UnreadMessagesCount = c.Messages.Count(m => m.SenderId != currentUser.Id && !m.IsRead)
                })
                .ToListAsync();

            return Result<List<ConversationDto>>.Success(conversations);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت مکالمات اخیر {UserId}", currentUser.Id);
            return Result<List<ConversationDto>>.GeneralFailure("خطا در دریافت مکالمات اخیر");
        }
    }

    public async Task<Result<List<MessageDto>>> GetConversationMessagesAsync(int conversationId, UserAccount currentUser, int page = 1, int pageSize = 50)
    {
        try
        {
            // Check if user is part of this conversation
            var conversation = await _conversationRepository.Query()
                .Where(c => c.Id == conversationId && (c.User1Id == currentUser.Id || c.User2Id == currentUser.Id))
                .FirstOrDefaultAsync();

            if (conversation == null)
                return Result<List<MessageDto>>.NotFound("مکالمه یافت نشد");

            var messages = await _messageRepository.Query()
                .Where(m => m.ConversationId == conversationId)
                .Include(m => m.Sender)
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    SenderName = m.Sender.UserName,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead,
                    ReadAt = m.ReadAt,
                    IsEdited = m.IsEdited,
                    EditedAt = m.EditedAt
                })
                .ToListAsync();

            return Result<List<MessageDto>>.Success(messages.OrderBy(m => m.SentAt).ToList());
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در دریافت پیام‌های مکالمه {ConversationId} برای کاربر {UserId}", conversationId, currentUser.Id);
            return Result<List<MessageDto>>.GeneralFailure("خطا در دریافت پیام‌های مکالمه");
        }
    }

    public async Task<Result> BlockUserAsync(BlockUserDto model, UserAccount currentUser)
    {
        try
        {
            var conversation = await _conversationRepository.Query()
                .Where(c => (c.User1Id == currentUser.Id && c.User2Id == model.UserId) ||
                           (c.User1Id == model.UserId && c.User2Id == currentUser.Id))
                .FirstOrDefaultAsync();

            if (conversation == null)
            {
                // Create conversation if it doesn't exist
                var createResult = await GetOrCreateConversationAsync(model.UserId, currentUser);
                if (!createResult.IsSuccess)
                    return Result.GeneralFailure(createResult.Message);

                conversation = await _conversationRepository.GetByIdAsync(createResult.Value.Id);
            }

            // Update block status
            if (conversation.User1Id == currentUser.Id)
                conversation.IsUser1Blocked = model.IsBlocked;
            else
                conversation.IsUser2Blocked = model.IsBlocked;

            await _conversationRepository.UpdateAsync(conversation);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در بلاک کردن کاربر {UserId} توسط {CurrentUserId}", model.UserId, currentUser.Id);
            return Result.GeneralFailure("خطا در بلاک کردن کاربر");
        }
    }

    public async Task<Result> UnblockUserAsync(int userId, UserAccount currentUser)
    {
        try
        {
            var conversation = await _conversationRepository.Query()
                .Where(c => (c.User1Id == currentUser.Id && c.User2Id == userId) ||
                           (c.User1Id == userId && c.User2Id == currentUser.Id))
                .FirstOrDefaultAsync();

            if (conversation == null)
                return Result.NotFound("مکالمه یافت نشد");

            // Update block status
            if (conversation.User1Id == currentUser.Id)
                conversation.IsUser1Blocked = false;
            else
                conversation.IsUser2Blocked = false;

            await _conversationRepository.UpdateAsync(conversation);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در آنبلاک کردن کاربر {UserId} توسط {CurrentUserId}", userId, currentUser.Id);
            return Result.GeneralFailure("خطا در آنبلاک کردن کاربر");
        }
    }

    public async Task<Result> MarkMessagesAsReadAsync(int conversationId, UserAccount currentUser)
    {
        try
        {
            var messages = await _messageRepository.Query()
                .Where(m => m.ConversationId == conversationId && m.SenderId != currentUser.Id && !m.IsRead)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
            }

            _messageRepository.UpdateRange(messages);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در علامت‌گذاری پیام‌ها به عنوان خوانده شده {ConversationId} برای کاربر {UserId}", conversationId, currentUser.Id);
            return Result.GeneralFailure("خطا در علامت‌گذاری پیام‌ها به عنوان خوانده شده");
        }
    }

    public async Task<Result<ConversationDto>> GetOrCreateConversationAsync(int otherUserId, UserAccount currentUser)
    {
        try
        {
            // Check if other user exists
            var otherUser = await _userAccountRepository.GetByIdAsync(otherUserId);
            if (otherUser == null)
                return Result<ConversationDto>.NotFound("کاربر یافت نشد");

            // Try to find existing conversation
            var existingConversation = await _conversationRepository.Query()
                .Where(c => (c.User1Id == currentUser.Id && c.User2Id == otherUserId) ||
                           (c.User1Id == otherUserId && c.User2Id == currentUser.Id))
                .Include(c => c.User1)
                .Include(c => c.User2)
                .FirstOrDefaultAsync();

            if (existingConversation != null)
            {
                var existingDto = new ConversationDto
                {
                    Id = existingConversation.Id,
                    User1Id = existingConversation.User1Id,
                    User2Id = existingConversation.User2Id,
                    User1Name = existingConversation.User1.UserName,
                    User2Name = existingConversation.User2.UserName,
                    LastMessageAt = existingConversation.LastMessageAt,
                    IsUser1Blocked = existingConversation.IsUser1Blocked,
                    IsUser2Blocked = existingConversation.IsUser2Blocked,
                    UnreadMessagesCount = 0
                };
                return Result<ConversationDto>.Success(existingDto);
            }

            // Create new conversation
            var newConversation = new Conversation
            {
                User1Id = Math.Min(currentUser.Id, otherUserId),
                User2Id = Math.Max(currentUser.Id, otherUserId),
                LastMessageAt = DateTime.UtcNow,
                IsUser1Blocked = false,
                IsUser2Blocked = false
            };

            await _conversationRepository.AddAsync(newConversation);
            await _unitOfWork.SaveChangesAsync();

            var newDto = new ConversationDto
            {
                Id = newConversation.Id,
                User1Id = newConversation.User1Id,
                User2Id = newConversation.User2Id,
                User1Name = newConversation.User1Id == currentUser.Id ? currentUser.UserName : otherUser.UserName,
                User2Name = newConversation.User2Id == currentUser.Id ? currentUser.UserName : otherUser.UserName,
                LastMessageAt = newConversation.LastMessageAt,
                IsUser1Blocked = newConversation.IsUser1Blocked,
                IsUser2Blocked = newConversation.IsUser2Blocked,
                UnreadMessagesCount = 0
            };

            return Result<ConversationDto>.Success(newDto);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "خطا در ایجاد یا دریافت مکالمه بین {CurrentUserId} و {OtherUserId}", currentUser.Id, otherUserId);
            return Result<ConversationDto>.GeneralFailure("خطا در ایجاد یا دریافت مکالمه");
        }
    }
}