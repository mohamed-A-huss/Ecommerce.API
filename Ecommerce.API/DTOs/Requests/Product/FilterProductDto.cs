namespace Ecommerce.API.DTOs.Requests.Product
{
    public class FilterProductDto
    {
        
        public string? Name { get; set; }

        
        public bool? Status { get; set; } 

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public decimal? Discount { get; set; }

        public int? CategoryId { get; set; }

        public int? BrandId { get; set; }
        public bool Trend { get; set; }=false;
    }
}
