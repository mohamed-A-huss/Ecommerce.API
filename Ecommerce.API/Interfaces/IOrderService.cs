using Ecommerce.API.DTOs.Requests.Order;
using Ecommerce.API.DTOs.Responses.Order;

namespace Ecommerce.API.Interfaces
{
    public interface IOrderService
    {
        Task<PaginatedOrderResponseDto> GetAll(FilterOrderDto filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<OrderItemDto?> GetByIdAsync(int id);
        Task<bool> Refund(int id);
        Task<OrderItemDto?> UpdateAsync(int id, UpdateOrderDto dto);
    }
}
