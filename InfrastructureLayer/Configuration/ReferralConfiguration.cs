using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class ReferralConfiguration : BaseEntityConfiguration<Referral>
    {
        public override void Configure(EntityTypeBuilder<Referral> builder)
        {
            base.Configure(builder);
        }
    }
}