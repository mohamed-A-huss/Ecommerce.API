namespace Ecommerce.API.DTOs.Responses.Category
{
    public class CategoryResponseDto
    {
        public IEnumerable<CategoryItemDto> Categories { get; set; } = null!;

        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }

        public string? Query { get; set; }
    }
}
