using Microsoft.AspNetCore.Identity;


namespace Ecommerce.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string? Address { get; set; }
        public string RefreshToken { get; set; } = string.Empty;

        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
