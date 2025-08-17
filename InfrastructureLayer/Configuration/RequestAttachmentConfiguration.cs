using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureLayer.Configuration
{
    public class RequestAttachmentConfiguration : BaseEntityConfiguration<RequestAttachment>
    {
        public override void Configure(EntityTypeBuilder<RequestAttachment> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.FilePath)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(x => x.FileType)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.AttachmentType)
                   .IsRequired()
                   .HasConversion<int>();

            builder.HasOne(x => x.Request)
                   .WithMany(x => x.Attachments)
                   .HasForeignKey(x => x.RequestId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}