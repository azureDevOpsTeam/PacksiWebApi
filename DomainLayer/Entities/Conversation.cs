using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities;

public class Conversation : BaseEntityModel, IAuditableEntity
{
    public int User1Id { get; set; }
    
    public int User2Id { get; set; }
    
    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;
    
    public bool IsUser1Blocked { get; set; } = false;
    
    public bool IsUser2Blocked { get; set; } = false;
    
    // Navigation Properties
    public UserAccount User1 { get; set; }
    
    public UserAccount User2 { get; set; }
    
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}