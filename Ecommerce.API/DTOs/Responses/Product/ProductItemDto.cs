namespace Ecommerce.API.DTOs.Responses.Product
{
    public class ProductItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string MainImg { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public double Rate { get; set; }
        public bool Status { get; set; }
        
        public string CategoryName { get; set; } = string.Empty;

        public string BrandName { get; set; } = string.Empty;



    }
}
