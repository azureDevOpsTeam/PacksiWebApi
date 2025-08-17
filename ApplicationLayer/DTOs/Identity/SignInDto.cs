namespace ApplicationLayer.DTOs.Identity
{
    public class SignInDto
    {
        public int ValidationMethod { get; set; }

        public string UserName { get; set; }

        public string PhonePrefix { get; set; }

        public string Password { get; set; }

        public int SecurityCode { get; set; }
    }
}