using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration;

public class SuggestionAttachmentConfiguration : BaseEntityConfiguration<SuggestionAttachment>
{
    public override void Configure(EntityTypeBuilder<SuggestionAttachment> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.Suggestion)
               .WithMany(x => x.SuggestionAttachments)
               .HasForeignKey(x => x.SuggestionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}