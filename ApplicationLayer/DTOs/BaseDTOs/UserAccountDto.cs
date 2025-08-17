using System.ComponentModel;

namespace ApplicationLayer.DTOs.BaseDTOs
{
    public class UserAccountDto
    {
        [DefaultValue(null)]
        public List<int> UserAccountIds { get; set; }
    }
}