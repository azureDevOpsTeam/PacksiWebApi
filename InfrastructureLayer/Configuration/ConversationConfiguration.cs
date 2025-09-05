using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class ConversationConfiguration : BaseEntityConfiguration<Conversation>
    {
        public override void Configure(EntityTypeBuilder<Conversation> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.LastMessageAt)
                .IsRequired();

            builder.Property(x => x.IsUser1Blocked)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.IsUser2Blocked)
                .IsRequired()
                .HasDefaultValue(false);

            // User1 relationship
            builder.HasOne(x => x.User1)
                   .WithMany(x => x.ConversationsAsUser1)
                   .HasForeignKey(x => x.User1Id)
                   .OnDelete(DeleteBehavior.Restrict);

            // User2 relationship
            builder.HasOne(x => x.User2)
                   .WithMany(x => x.ConversationsAsUser2)
                   .HasForeignKey(x => x.User2Id)
                   .OnDelete(DeleteBehavior.Restrict);

            // Ensure User1Id != User2Id
            builder.HasCheckConstraint("CK_Conversation_DifferentUsers", "[User1Id] != [User2Id]");

            // Create unique index for User1Id and User2Id combination
            builder.HasIndex(x => new { x.User1Id, x.User2Id })
                   .IsUnique()
                   .HasDatabaseName("IX_Conversation_Users");
        }
    }
}