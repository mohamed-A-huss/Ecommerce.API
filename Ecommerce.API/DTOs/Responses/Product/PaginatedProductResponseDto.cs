namespace Ecommerce.API.DTOs.Responses.Product
{
    public class PaginatedProductResponseDto
    {
        public IEnumerable<ProductItemDto> Products { get; set; } = [];

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }
    }
}
