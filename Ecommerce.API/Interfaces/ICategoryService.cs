
using Ecommerce.API.DTOs.Requests.Category;
using Ecommerce.API.DTOs.Responses.Category;

namespace Ecommerce.API.Interfaces
{
    public interface ICategoryService
    {
        Task<PaginatedCategoryResponseDto> GetAll(FilterCategoryDto filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<CategoryItemDto?> GetByIdAsync(int id);
        Task<CategoryItemDto?> CreateAsync(CreateCategoryDto dto);
        Task<CategoryItemDto?> UpdateAsync(int id, UpdateCategoryDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ChangeStatusAsync(int id);
    }
}
