using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class PostPricingConfiguration : BaseEntityConfiguration<PostPricing>
    {
        public override void Configure(EntityTypeBuilder<PostPricing> builder)
        {
            base.Configure(builder);

            builder.Property(p => p.PostType)
                .IsRequired();

            builder.Property(p => p.AdvertisementType)
                .IsRequired();

            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}