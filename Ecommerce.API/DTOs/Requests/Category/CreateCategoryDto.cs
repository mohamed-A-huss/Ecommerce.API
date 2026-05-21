namespace Ecommerce.API.DTOs.Requests.Category
{
    public class CreateCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
    }
}
