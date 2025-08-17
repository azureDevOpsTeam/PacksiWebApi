using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class InvitationConfiguration : BaseEntityConfiguration<Invitation>
    {
        public override void Configure(EntityTypeBuilder<Invitation> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Code)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.HasIndex(x => x.Code)
                   .IsUnique();

            builder.Property(x => x.UsageCount)
                   .HasDefaultValue(0);

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true);

            builder.HasOne(x => x.InviterUser)
                   .WithMany(u => u.Invitations)
                   .HasForeignKey(x => x.InviterUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}