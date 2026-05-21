namespace Ecommerce.API.DTOs.Requests.Product
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public IFormFile MainImg { get; set; } = null!;

        public string? Description { get; set; }
        public bool Status { get; set; } = true;

        [Required]
        public decimal Price { get; set; }

        public decimal Discount { get; set; } = 0;

        public int CategoryId { get; set; }

        public int BrandId { get; set; }
    }
}
