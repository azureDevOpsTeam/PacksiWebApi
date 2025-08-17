using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class CommentConfiguration : BaseEntityConfiguration<Comment>
    {
        public override void Configure(EntityTypeBuilder<Comment> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Content)
                .IsRequired()
                .HasMaxLength(1000);

            builder.HasOne(x => x.UserAccount)
                   .WithMany(x => x.Comments)
                   .HasForeignKey(x => x.UserAccountId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}