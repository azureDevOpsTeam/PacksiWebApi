using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.BusinessLogic.Interfaces.LiveChat;
using ApplicationLayer.DTOs.LiveChat;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using Microsoft.AspNetCore.SignalR;

namespace PresentationApp.Hubs;

public class ChatHub(IMiniAppServices miniAppServices, IUserAccountServices userAccountServices, ILiveChatServices liveChatServices,
 ILogger<ChatHub> logger, IUserContextService userContextService) : Hub
{
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;
    private readonly ILiveChatServices _liveChatServices = liveChatServices;
    private readonly ILogger<ChatHub> _logger = logger;
    private readonly IUserContextService _userContextService = userContextService;

    public override async Task OnConnectedAsync()
    {
        try
        {
            var validation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
            if (validation.IsFailure)
            {
                _logger.LogWarning("Telegram mini app validation failed for connection {ConnectionId}", Context.ConnectionId);
                Context.Abort();
                return;
            }

            var userAccount = await _userAccountServices.GetUserAccountByTelegramIdAsync(validation.Value.User.Id);
            if (userAccount.IsFailure)
            {
                _logger.LogWarning("No user account found for TelegramId {TelegramId}", validation.Value.User.Id);
                Context.Abort();
                return;
            }

            var userId = userAccount.Value.Id.ToString();
            Context.Items["UserId"] = userId;

            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            _logger.LogInformation("User {UserId} connected to chat hub with connection {ConnectionId}", userId, Context.ConnectionId);

            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in OnConnectedAsync for connection {ConnectionId}", Context.ConnectionId);
            Context.Abort();
        }
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        try
        {
            var userId = Context.Items["UserId"]?.ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
                _logger.LogInformation("User {UserId} disconnected from chat hub", userId);
            }
            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during OnDisconnectedAsync");
        }
    }

    public async Task SendMessage(SendMessageDto messageDto)
    {
        try
        {
            var userId = Context.Items["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                await Clients.Caller.SendAsync("Error", "کاربر احراز هویت نشده است");
                return;
            }

            var currentUser = new UserAccount { Id = int.Parse(userId) };
            var result = await _liveChatServices.SendMessageAsync(messageDto, currentUser);

            if (result.IsSuccess)
            {
                // ارسال به خود کاربر (فرستنده)
                await Clients.Group($"User_{currentUser.Id}").SendAsync("MessageSent", result.Value);

                // ارسال به گیرنده
                await Clients.Group($"User_{messageDto.ReceiverId}").SendAsync("MessageReceived", result.Value);
            }
            else
            {
                await Clients.Caller.SendAsync("Error", result.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while sending message via SignalR");
            await Clients.Caller.SendAsync("Error", "خطا در ارسال پیام");
        }
    }

    public async Task JoinConversation(int conversationId)
    {
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
            _logger.LogInformation("User {UserId} joined conversation {ConversationId}", Context.Items["UserId"], conversationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while joining conversation {ConversationId}", conversationId);
        }
    }

    public async Task LeaveConversation(int conversationId)
    {
        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
            _logger.LogInformation("User {UserId} left conversation {ConversationId}", Context.Items["UserId"], conversationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while leaving conversation {ConversationId}", conversationId);
        }
    }

    public async Task MarkMessagesAsRead(int conversationId)
    {
        try
        {
            var userId = Context.Items["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
                return;

            var currentUser = new UserAccount { Id = int.Parse(userId) };
            var result = await _liveChatServices.MarkMessagesAsReadAsync(conversationId, currentUser);

            if (result.IsSuccess)
            {
                await Clients.Group($"Conversation_{conversationId}")
                    .SendAsync("MessagesMarkedAsRead", conversationId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while marking messages as read for conversation {ConversationId}", conversationId);
        }
    }
}