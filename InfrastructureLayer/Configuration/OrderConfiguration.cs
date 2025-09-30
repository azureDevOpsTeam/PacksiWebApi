using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class OrderConfiguration : BaseEntityConfiguration<Order>
{
    public override void Configure(EntityTypeBuilder<Order> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.Suggestion)
               .WithMany(x => x.Orders)
               .HasForeignKey(x => x.SuggestionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}