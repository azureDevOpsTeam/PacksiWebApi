using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class RequestItemTypeConfiguration : BaseEntityConfiguration<RequestItemType>
    {
        public override void Configure(EntityTypeBuilder<RequestItemType> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.Request)
                   .WithMany(x => x.RequestItemTypes)
                   .HasForeignKey(x => x.RequestId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.ItemType).IsRequired();
        }
    }
}