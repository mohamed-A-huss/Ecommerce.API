

namespace Ecommerce.API.Interfaces
{
    public interface IBrandService
    {
        Task<PaginatedBrandResponseDto> GetAll(FilterBrandDto filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<Brand?> GetByIdAsync(int id);
        Task<Brand?> CreateAsync(CreateBrandDto dto);
        Task<Brand?> UpdateAsync(int id, UpdateBrandDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ChangeStatusAsync(int id);
    }
}
