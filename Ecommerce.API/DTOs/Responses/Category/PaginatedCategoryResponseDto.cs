namespace Ecommerce.API.DTOs.Responses.Category
{
    public class PaginatedCategoryResponseDto
    {
        public IEnumerable<CategoryItemDto> Categories { get; set; } = [];

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }
    }
}
