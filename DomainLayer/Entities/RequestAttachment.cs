using DomainLayer.Common.BaseEntities;

namespace DomainLayer.Entities
{
    public class RequestAttachment : BaseEntityModel, IAuditableEntity
    {
        public int RequestId { get; set; }

        public string FilePath { get; set; }

        public string FileType { get; set; } // image/jpeg, image/png, pdf, etc.

        public int AttachmentType { get; set; } // مثلاً Ticket, Identity, ItemImage

        public Request Request { get; set; }
    }
}