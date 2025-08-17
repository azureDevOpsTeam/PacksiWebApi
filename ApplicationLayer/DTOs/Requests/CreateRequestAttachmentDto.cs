using ApplicationLayer.Extensions.SmartEnums;

namespace ApplicationLayer.DTOs.Requests
{
    public class CreateRequestAttachmentDto
    {
        public string FilePath { get; set; }

        public string FileType { get; set; }

        public AttachmentTypeEnum AttachmentType { get; set; }
    }
}