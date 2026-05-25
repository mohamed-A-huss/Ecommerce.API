namespace Ecommerce.API.DTOs.Responses.User
{
    public class ApplicationUserResponseDto
    {
        public List<UserWithRoleDto> Users { get; set; } = new();

        public double TotalPages { get; set; }

        public int CurrentPage { get; set; }
    }
}
