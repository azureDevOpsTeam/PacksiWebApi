using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class Message : BaseEntityModel, IAuditableEntity
    {
        public int ConversationId { get; set; }
        
        public int SenderId { get; set; }
        
        public string Content { get; set; }
        
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        
        public bool IsRead { get; set; } = false;
        
        public DateTime? ReadAt { get; set; }
        
        public bool IsEdited { get; set; } = false;
        
        public DateTime? EditedAt { get; set; }
        
        // Navigation Properties
        public Conversation Conversation { get; set; }
        
        public UserAccount Sender { get; set; }
    }
}