using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class RequestConfiguration : BaseEntityConfiguration<Request>
    {
        public override void Configure(EntityTypeBuilder<Request> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.DepartureDate)
                   .IsRequired();

            builder.Property(x => x.RequestType)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(x => x.Description)
                   .HasMaxLength(1000);

            builder.Property(x => x.SuggestedPrice)
                   .HasPrecision(18, 2);

            builder.HasOne(x => x.UserAccount)
                   .WithMany()
                   .HasForeignKey(x => x.UserAccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OriginCity)
                   .WithMany()
                   .HasForeignKey(x => x.OriginCityId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DestinationCity)
                   .WithMany()
                   .HasForeignKey(x => x.DestinationCityId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}