namespace Ecommerce.API.DTOs.Requests.Brand
{
    public class CreateBrandDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public IFormFile Logo { get; set; } = null!;
        public bool Status { get; set; } = true;
    }
}
