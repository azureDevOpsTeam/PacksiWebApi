using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class RequestAvailableOriginConfiguration : BaseEntityConfiguration<RequestAvailableOrigin>
    {
        public override void Configure(EntityTypeBuilder<RequestAvailableOrigin> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.LocationDescription)
                   .HasMaxLength(500);

            builder.HasOne(x => x.Request)
                   .WithMany(x => x.AvailableOrigins)
                   .HasForeignKey(x => x.RequestId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.City)
                   .WithMany()
                   .HasForeignKey(x => x.CityId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}