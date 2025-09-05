using System.ComponentModel.DataAnnotations;

namespace ApplicationLayer.DTOs.LiveChat
{
    public class BlockUserDto
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public bool IsBlocked { get; set; }
    }
}