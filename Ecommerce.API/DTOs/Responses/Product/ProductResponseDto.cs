namespace Ecommerce.API.DTOs.Responses.Product
{
    public class ProductResponseDto
    {
        public IEnumerable<ProductItemDto> Products { get; set; } = null!;

        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }

        public string? Query { get; set; }
    }
}
