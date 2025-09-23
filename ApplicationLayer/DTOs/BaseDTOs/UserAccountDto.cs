using System.ComponentModel;

namespace ApplicationLayer.DTOs.BaseDTOs
{
    public class UserAccountIdsDto
    {
        [DefaultValue(null)]
        public List<int> UserAccountIds { get; set; }
    }
}