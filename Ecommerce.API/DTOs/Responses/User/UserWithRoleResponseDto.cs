using Microsoft.AspNetCore.Identity;

namespace Ecommerce.API.DTOs.Responses.User
{
    public class UserWithRoleResponseDto
    {
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public string RoleName { get; set; } = string.Empty;
        public IEnumerable<IdentityRole> IdentityRoles { get; set; } = [];
    
}
}
