namespace Ecommerce.API.DTOs.Responses.Brand
{
    public class PaginatedBrandResponseDto
    {
        public IEnumerable<BrandItemDto> Brands { get; set; } = [];

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }
    }
}
