using DomainLayer.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using PresentationApp.Hubs;
using Microsoft.Extensions.Logging;

namespace PresentationApp.EventHandlers;

public class MessageSentEventHandler(
    IHubContext<ChatHub> hubContext,
    ILogger<MessageSentEventHandler> logger) : INotificationHandler<MessageSentEvent>
{
    private readonly IHubContext<ChatHub> _hubContext = hubContext;
    private readonly ILogger<MessageSentEventHandler> _logger = logger;

    public async Task Handle(MessageSentEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing MessageSentEvent for message {MessageId} from {SenderId} to {ReceiverId}", 
                notification.MessageId, notification.SenderId, notification.ReceiverId);

            // ارسال پیام به فرستنده (تأیید ارسال)
            await _hubContext.Clients.Group($"User_{notification.SenderId}")
                .SendAsync("MessageSent", new
                {
                    MessageId = notification.MessageId,
                    ConversationId = notification.ConversationId,
                    Content = notification.Content,
                    SentAt = notification.SentAt,
                    SenderId = notification.SenderId,
                    ReceiverId = notification.ReceiverId
                }, cancellationToken);

            // ارسال پیام به گیرنده (دریافت پیام جدید)
            await _hubContext.Clients.Group($"User_{notification.ReceiverId}")
                .SendAsync("MessageReceived", new
                {
                    MessageId = notification.MessageId,
                    ConversationId = notification.ConversationId,
                    Content = notification.Content,
                    SentAt = notification.SentAt,
                    SenderId = notification.SenderId,
                    ReceiverId = notification.ReceiverId
                }, cancellationToken);

            _logger.LogInformation("MessageSentEvent processed successfully for message {MessageId}", notification.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MessageSentEvent for message {MessageId}", notification.MessageId);
            throw;
        }
    }
}