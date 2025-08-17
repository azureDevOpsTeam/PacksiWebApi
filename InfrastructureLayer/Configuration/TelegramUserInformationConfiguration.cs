using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class TelegramUserInformationConfiguration : BaseEntityConfiguration<TelegramUserInformation>
    {
        public override void Configure(EntityTypeBuilder<TelegramUserInformation> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.UserAccount)
                   .WithMany(x => x.TelegramUserInformations)
                   .HasForeignKey(x => x.UserAccountId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}