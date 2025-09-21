using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class WalletConfiguration : BaseEntityConfiguration<Wallet>
{
    public override void Configure(EntityTypeBuilder<Wallet> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.UserAccount)
               .WithMany(x => x.Wallets)
               .HasForeignKey(x => x.UserAccountId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(w => w.Currency).HasMaxLength(3).IsRequired();
        builder.Property(w => w.Balance).HasColumnType("decimal(28,2)");
        builder.Property(w => w.Reserved).HasColumnType("decimal(28,2)");
        builder.Property(w => w.RowVersion).IsRowVersion();

        builder.HasIndex(w => new { w.UserAccountId, w.Currency }).IsUnique();
    }
}