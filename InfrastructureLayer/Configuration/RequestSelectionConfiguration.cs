using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class RequestSelectionConfiguration : BaseEntityConfiguration<RequestSelection>
    {
        public override void Configure(EntityTypeBuilder<RequestSelection> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.UserAccount)
                   .WithMany(x => x.RequestSelections)
                   .HasForeignKey(x => x.UserAccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Request)
                   .WithMany(x => x.RequestSelections)
                   .HasForeignKey(x => x.RequestId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}