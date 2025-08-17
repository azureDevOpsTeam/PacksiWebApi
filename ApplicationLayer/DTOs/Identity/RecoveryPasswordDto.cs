namespace ApplicationLayer.DTOs.Identity
{
    public class RecoveryPasswordDto
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public int SecurityCode { get; set; }
    }
}