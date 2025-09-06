using DomainLayer.Common.Events;

namespace DomainLayer.Events
{
    public class MessageSentEvent : IDomainEvent
    {
        public int MessageId { get; }
        public int ConversationId { get; }
        public int SenderId { get; }
        public int ReceiverId { get; }
        public string Content { get; }
        public DateTime SentAt { get; }
        public DateTime OccurredOn { get; }

        public MessageSentEvent(int messageId, int conversationId, int senderId, int receiverId, string content, DateTime sentAt)
        {
            MessageId = messageId;
            ConversationId = conversationId;
            SenderId = senderId;
            ReceiverId = receiverId;
            Content = content;
            SentAt = sentAt;
            OccurredOn = DateTime.UtcNow;
        }
    }
}