using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class AdScheduleConfiguration : BaseEntityConfiguration<AdSchedule>
    {
        public override void Configure(EntityTypeBuilder<AdSchedule> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.DayOfWeek)
                .IsRequired();

            builder.Property(s => s.FromTime)
                .IsRequired();

            builder.Property(s => s.ToTime)
                .IsRequired();

            builder.HasOne(s => s.Advertisement)
                .WithMany(a => a.Schedules)
                .HasForeignKey(s => s.AdvertisementId);
        }
    }
}