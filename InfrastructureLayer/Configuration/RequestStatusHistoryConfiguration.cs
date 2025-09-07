using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class RequestStatusHistoryConfiguration : BaseEntityConfiguration<RequestStatusHistory>
{
    public override void Configure(EntityTypeBuilder<RequestStatusHistory> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.Suggestion)
               .WithMany(x => x.RequestStatusHistories)
               .HasForeignKey(x => x.RequestSelectionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.UserAccount)
               .WithMany(x => x.StatusHistories)
               .HasForeignKey(x => x.UserAccountId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);

        builder.Property(x => x.Comment)
               .HasMaxLength(1000);
    }
}