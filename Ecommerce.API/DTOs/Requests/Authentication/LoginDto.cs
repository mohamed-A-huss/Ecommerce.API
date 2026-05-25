namespace Ecommerce.API.DTOs.Requests.Authentication
{
    public class LoginDto
    {
        [Required]
        public string UserNameOrEmail { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } // by default : 14 day
    }
}
