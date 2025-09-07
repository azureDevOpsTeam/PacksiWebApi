using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class SuggestionConfiguration : BaseEntityConfiguration<Suggestion>
{
    public override void Configure(EntityTypeBuilder<Suggestion> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.UserAccount)
               .WithMany(x => x.Suggestions)
               .HasForeignKey(x => x.UserAccountId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Request)
               .WithMany(x => x.Suggestions)
               .HasForeignKey(x => x.RequestId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}