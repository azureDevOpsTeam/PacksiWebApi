using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class UserRatingConfiguration : BaseEntityConfiguration<UserRating>
{
    public override void Configure(EntityTypeBuilder<UserRating> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.RateeUserAccount)
               .WithMany(x => x.RateeUserAccounts)
               .HasForeignKey(x => x.RateeUserAccountId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RaterUserAccount)
               .WithMany(x => x.RaterUserAccounts)
               .HasForeignKey(x => x.RaterUserAccountId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Request)
               .WithMany(x => x.UserRatings)
               .HasForeignKey(x => x.RequestId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}