namespace ApplicationLayer.DTOs.RefreshTokens
{
    public class TokenRequestDto
    {
        public string AccessTokens { get; set; }

        public string RefreshToken { get; set; }
    }
}