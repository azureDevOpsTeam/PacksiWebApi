using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class UserAccountConfiguration : BaseEntityConfiguration<UserAccount>
    {
        public override void Configure(EntityTypeBuilder<UserAccount> builder)
        {
            base.Configure(builder);

            builder.Property(current => current.UserName).HasMaxLength(100);
            builder.Property(current => current.Password).HasMaxLength(128);
            builder.Property(current => current.Email).HasMaxLength(200);

            builder.HasOne(u => u.InvitedByManager).WithMany(m => m.InvitedUsers).HasForeignKey(u => u.InvitedByManagerId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}