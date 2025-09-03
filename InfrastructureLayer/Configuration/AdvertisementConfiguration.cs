using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class AdvertisementConfiguration : BaseEntityConfiguration<Advertisement>
    {
        public override void Configure(EntityTypeBuilder<Advertisement> builder)
        {
            base.Configure(builder);

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Content)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(a => a.MediaUrl)
                .HasMaxLength(500);

            builder.Property(a => a.PostType)
                .IsRequired();

            builder.Property(a => a.AdvertisementType)
                .IsRequired();

            builder.Property(a => a.Price)
                .HasColumnType("decimal(18,2)");

            builder.HasMany(a => a.Schedules)
                .WithOne(s => s.Advertisement)
                .HasForeignKey(s => s.AdvertisementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.UserAccount)
                .WithMany(u => u.Advertisements)
                .HasForeignKey(a => a.UserAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(a => a.UserAccountId).IsRequired();
        }
    }
}