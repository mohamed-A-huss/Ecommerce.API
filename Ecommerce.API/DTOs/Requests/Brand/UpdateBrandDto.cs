namespace Ecommerce.API.DTOs.Requests.Brand
{
    public class UpdateBrandDto
    {
        public string? Name { get; set; }
        public IFormFile? Logo { get; set; }
        public bool? Status { get; set; }
    }
}
