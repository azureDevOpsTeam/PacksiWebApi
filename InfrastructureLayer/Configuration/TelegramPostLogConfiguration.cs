using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class TelegramPostLogConfiguration : BaseEntityConfiguration<TelegramPostLog>
    {
        public override void Configure(EntityTypeBuilder<TelegramPostLog> builder)
        {
            base.Configure(builder);

            builder.Property(p => p.SentAt)
                .IsRequired();

            builder.Property(p => p.IsSuccessful)
                .IsRequired();

            builder.Property(p => p.TelegramMessageId)
                .HasMaxLength(100);

            builder.Property(p => p.ErrorMessage)
                .HasMaxLength(1000);

            builder.HasOne(p => p.Advertisement)
                .WithMany()
                .HasForeignKey(p => p.AdvertisementId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}