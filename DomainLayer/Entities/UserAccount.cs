using DomainLayer.Common.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Entities
{
    public class UserAccount : BaseEntityModel, IAuditableEntity
    {
        public long? TelegramId { get; set; }

        public string TelegramUserName { get; set; }

        public string Avatar { get; set; }

        public long? ReferredByUserId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public bool ConfirmEmail { get; set; }

        public string PhonePrefix { get; set; }

        public string PhoneNumber { get; set; }

        public bool ConfirmPhoneNumber { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public string SecurityStamp { get; set; }

        public string InviteCode { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public int? SecurityCode { get; set; }

        public DateTime? ExpireSecurityCode { get; set; }

        public int FailedAttempts { get; set; }

        public DateTime? LockedTime { get; set; }

        public ICollection<UserProfile> UserProfiles { get; set; } = [];

        public ICollection<UserRole> UserRoles { get; set; } = [];

        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

        public int? InvitedByManagerId { get; set; }

        public UserAccount InvitedByManager { get; set; }

        public ICollection<UserAccount> InvitedUsers { get; set; } = [];

        public ICollection<Invitation> Invitations { get; set; } = [];

        public ICollection<TelegramUserInformation> TelegramUserInformations { get; set; } = [];

        public ICollection<RequestStatusHistory> StatusHistories { get; set; } = [];

        public ICollection<UserPreferredLocation> UserPreferredLocations { get; set; } = [];

        public ICollection<Advertisement> Advertisements { get; set; } = [];

        public ICollection<Comment> Comments { get; set; } = [];

        public ICollection<Conversation> ConversationsAsUser1 { get; set; } = [];

        public ICollection<Conversation> ConversationsAsUser2 { get; set; } = [];

        public ICollection<Message> SentMessages { get; set; } = [];

        public ICollection<Suggestion> Suggestions { get; set; } = [];

        public ICollection<UserRating> RateeUserAccounts { get; set; }

        public ICollection<UserRating> RaterUserAccounts { get; set; }
    }
}