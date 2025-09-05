using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class MessageConfiguration : BaseEntityConfiguration<Message>
    {
        public override void Configure(EntityTypeBuilder<Message> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Content)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.SentAt)
                .IsRequired();

            builder.Property(x => x.IsRead)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.IsEdited)
                .IsRequired()
                .HasDefaultValue(false);

            // Conversation relationship
            builder.HasOne(x => x.Conversation)
                   .WithMany(x => x.Messages)
                   .HasForeignKey(x => x.ConversationId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Sender relationship
            builder.HasOne(x => x.Sender)
                   .WithMany(x => x.SentMessages)
                   .HasForeignKey(x => x.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Index for better query performance
            builder.HasIndex(x => new { x.ConversationId, x.SentAt })
                   .HasDatabaseName("IX_Message_Conversation_SentAt");

            builder.HasIndex(x => x.SenderId)
                   .HasDatabaseName("IX_Message_SenderId");
        }
    }
}