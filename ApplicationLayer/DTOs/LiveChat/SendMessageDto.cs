using System.ComponentModel.DataAnnotations;

namespace ApplicationLayer.DTOs.LiveChat
{
    public class SendMessageDto
    {
        [Required]
        public int ReceiverId { get; set; }
        
        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public string Content { get; set; }
    }
}