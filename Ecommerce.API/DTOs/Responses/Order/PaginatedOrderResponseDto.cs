namespace Ecommerce.API.DTOs.Responses.Order
{
    public class PaginatedOrderResponseDto
    {
        public IEnumerable<OrderItemDto> Orders { get; set; } = [];

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }
    }
}
