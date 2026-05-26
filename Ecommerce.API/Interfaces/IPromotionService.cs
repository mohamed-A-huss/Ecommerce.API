using Ecommerce.API.DTOs.Requests.Promotion;
using Ecommerce.API.DTOs.Responses.Promotion;

namespace Ecommerce.API.Interfaces
{
    public interface IPromotionService
    {
        Task<PaginatedPromotionResponseDto> GetAll(FilterPromotionDto filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<PromotionItemDto?> CreateAsync(CreatePromotionDto dto);
        Task<PromotionItemDto?> GetByIdAsync(int id);
        Task<PromotionItemDto?> UpdateAsync(int id, UpdatePromotionDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
