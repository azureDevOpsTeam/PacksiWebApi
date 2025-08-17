using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class RequestAvailableDestinationConfiguration : BaseEntityConfiguration<RequestAvailableDestination>
    {
        public override void Configure(EntityTypeBuilder<RequestAvailableDestination> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.LocationDescription)
                   .HasMaxLength(500);

            builder.HasOne(x => x.Request)
                   .WithMany(x => x.AvailableDestinations)
                   .HasForeignKey(x => x.RequestId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.City)
                   .WithMany()
                   .HasForeignKey(x => x.CityId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}