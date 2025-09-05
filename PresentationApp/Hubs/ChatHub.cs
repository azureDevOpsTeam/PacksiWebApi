using ApplicationLayer.BusinessLogic.Interfaces;
using ApplicationLayer.BusinessLogic.Interfaces.LiveChat;
using ApplicationLayer.DTOs.LiveChat;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using Microsoft.AspNetCore.SignalR;

namespace PresentationApp.Hubs;

public class ChatHub(IMiniAppServices miniAppServices, IUserAccountServices userAccountServices, ILiveChatServices liveChatServices, ILogger<ChatHub> logger, IUserContextService userContextService) : Hub
{
    private readonly IMiniAppServices _miniAppServices = miniAppServices;
    private readonly IUserAccountServices _userAccountServices = userAccountServices;
    private readonly ILiveChatServices _liveChatServices = liveChatServices;
    private readonly ILogger<ChatHub> _logger = logger;
    private readonly IUserContextService _userContextService = userContextService;

    public override async Task OnConnectedAsync()
    {
        var validation = await _miniAppServices.ValidateTelegramMiniAppUserAsync();
        if (validation.IsFailure)
            return;

        var userAccount = await _userAccountServices.GetUserAccountByTelegramIdAsync(validation.Value.User.Id);
        if (userAccount.IsFailure)
            return;
        var userId = userAccount.Value.Id.ToString();

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            _logger.LogInformation($"User {userId} connected to chat hub");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
            _logger.LogInformation($"User {userId} disconnected from chat hub");
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(SendMessageDto messageDto)
    {
        try
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                await Clients.Caller.SendAsync("Error", "کاربر احراز هویت نشده است");
                return;
            }

            var currentUser = new UserAccount { Id = _userContextService.UserId.Value };
            var result = await _liveChatServices.SendMessageAsync(messageDto, currentUser);

            if (result.IsSuccess)
            {
                // Send to sender
                await Clients.Group($"User_{currentUser.Id}").SendAsync("MessageSent", result.Value);

                // Send to receiver
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
            _logger.LogInformation($"User {Context.UserIdentifier} joined conversation {conversationId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while joining conversation {conversationId}");
        }
    }

    public async Task LeaveConversation(int conversationId)
    {
        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
            _logger.LogInformation($"User {Context.UserIdentifier} left conversation {conversationId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while leaving conversation {conversationId}");
        }
    }

    public async Task MarkMessagesAsRead(int conversationId)
    {
        try
        {
            var currentUser = new UserAccount { Id = _userContextService.UserId.Value };
            var result = await _liveChatServices.MarkMessagesAsReadAsync(conversationId, currentUser);
            if (result.IsSuccess)
            {
                await Clients.Group($"Conversation_{conversationId}").SendAsync("MessagesMarkedAsRead", conversationId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while marking messages as read for conversation {conversationId}");
        }
    }
}