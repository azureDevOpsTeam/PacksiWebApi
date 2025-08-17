namespace ApplicationLayer.DTOs.User
{
    public class InviteUserDto
    {
        public int? MaxUsageCount { get; set; }

        public DateTime? ExpireDate { get; set; }
    }
}