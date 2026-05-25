namespace Ecommerce.API.DTOs.Responses.Authentication
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }
    }
}
