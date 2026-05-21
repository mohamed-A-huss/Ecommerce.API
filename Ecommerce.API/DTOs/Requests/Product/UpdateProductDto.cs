namespace Ecommerce.API.DTOs.Requests.Product
{
    public class UpdateProductDto
    {
        public string? Name { get; set; } 

        public IFormFile? MainImg { get; set; }

        public string? Description { get; set; }
        public bool? Status { get; set; } 

        public decimal? Price { get; set; }

        public decimal? Discount { get; set; }

        public int? CategoryId { get; set; }

        public int? BrandId { get; set; }
    }
}
