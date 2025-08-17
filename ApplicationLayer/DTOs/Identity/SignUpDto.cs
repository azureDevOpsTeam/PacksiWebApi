namespace ApplicationLayer.DTOs.Identity
{
    public class SignUpDto
    {
        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string PhonePrefix { get; set; }

        public string PhoneNumber { get; set; }

        public string Password { get; set; }

        public string InviteCode { get; set; }
    }
}