namespace ApplicationLayer.DTOs.LiveChat
{
    public class ConversationDto
    {
        public int Id { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public string User1Name { get; set; }
        public string User2Name { get; set; }
        public DateTime LastMessageAt { get; set; }
        public string LastMessageContent { get; set; }
        public bool IsUser1Blocked { get; set; }
        public bool IsUser2Blocked { get; set; }
        public int UnreadMessagesCount { get; set; }
    }
}