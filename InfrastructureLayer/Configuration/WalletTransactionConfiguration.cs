using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

internal class WalletTransactionConfiguration : BaseEntityConfiguration<WalletTransaction>
{
    public override void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.Wallet)
               .WithMany(x => x.WalletTransactions)
               .HasForeignKey(x => x.WalletId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(t => t.Amount).HasColumnType("decimal(28,2)");
        builder.Property(t => t.BalanceAfter).HasColumnType("decimal(28,2)");
    }
}