using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class RequestStatusHistoryConfiguration : BaseEntityConfiguration<RequestStatusHistory>
    {
        public override void Configure(EntityTypeBuilder<RequestStatusHistory> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.Request)
                   .WithMany(x => x.StatusHistories)
                   .HasForeignKey(x => x.RequestId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ChangedByUser)
                   .WithMany()
                   .HasForeignKey(x => x.UserAccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Comment)
                   .HasMaxLength(1000);
        }
    }
}