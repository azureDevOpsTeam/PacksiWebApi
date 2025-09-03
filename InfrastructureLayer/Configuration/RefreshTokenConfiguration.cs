using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class RefreshTokenConfiguration : BaseEntityConfiguration<RefreshToken>
    {
        public override void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            base.Configure(builder);

            builder.Property(row => row.Token).IsRequired();
            builder.Property(row => row.JwtId).IsRequired();

            builder.HasOne(x => x.UserAccount)
                   .WithMany(x => x.RefreshTokens)
                   .HasForeignKey(x => x.UserAccountId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}