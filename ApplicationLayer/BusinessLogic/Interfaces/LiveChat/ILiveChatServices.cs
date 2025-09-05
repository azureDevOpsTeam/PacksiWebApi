using ApplicationLayer.DTOs;
using ApplicationLayer.DTOs.LiveChat;
using DomainLayer.Entities;

namespace ApplicationLayer.BusinessLogic.Interfaces.LiveChat;

public interface ILiveChatServices
{
    Task<Result<List<ChatListDto>>> GetUsersListAsync(UserAccount currentUser);
    
    Task<Result<MessageDto>> SendMessageAsync(SendMessageDto model, UserAccount sender);
    
    Task<Result<List<ConversationDto>>> GetRecentConversationsAsync(UserAccount currentUser);
    
    Task<Result<List<MessageDto>>> GetConversationMessagesAsync(int conversationId, UserAccount currentUser, int page = 1, int pageSize = 50);
    
    Task<Result> BlockUserAsync(BlockUserDto model, UserAccount currentUser);
    
    Task<Result> UnblockUserAsync(int userId, UserAccount currentUser);
    
    Task<Result> MarkMessagesAsReadAsync(int conversationId, UserAccount currentUser);
    
    Task<Result<ConversationDto>> GetOrCreateConversationAsync(int otherUserId, UserAccount currentUser);
}