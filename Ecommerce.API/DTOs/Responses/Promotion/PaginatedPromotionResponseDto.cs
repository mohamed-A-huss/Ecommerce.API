namespace Ecommerce.API.DTOs.Responses.Promotion
{
    public class PaginatedPromotionResponseDto
    {
        public IEnumerable<PromotionItemDto> Promotions { get; set; } = [];

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }
        
        public string? Query { get; set; }
    }
}
