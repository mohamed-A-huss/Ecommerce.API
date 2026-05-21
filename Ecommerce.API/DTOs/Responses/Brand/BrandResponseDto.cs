namespace Ecommerce.API.DTOs.Responses.Brand
{
    public class BrandResponseDto
    {
        public IEnumerable<BrandItemDto> Brands { get; set; } = null!;

        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }

        public string? Query { get; set; }
    }
}
