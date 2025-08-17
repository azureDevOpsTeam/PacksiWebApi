using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class UserPreferredLocationConfiguration : BaseEntityConfiguration<UserPreferredLocation>
    {
        public override void Configure(EntityTypeBuilder<UserPreferredLocation> builder)
        {
            base.Configure(builder);

            builder.HasOne(current => current.UserAccount)
                .WithMany(current => current.UserPreferredLocations)
                .HasForeignKey(current => current.UserAccountId);

            builder.HasOne(current => current.Country)
                .WithMany(current => current.UserPreferredLocations)
                .HasForeignKey(current => current.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(current => current.City)
                .WithMany(current => current.UserPreferredLocations)
                .HasForeignKey(current => current.CityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}