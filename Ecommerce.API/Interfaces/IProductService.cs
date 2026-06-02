
namespace Ecommerce.API.Interfaces
{
    public interface IProductService
    {
        Task<PaginatedProductResponseDto> GetAll(FilterProductDto filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<ProductItemDto?> GetByIdAsync(int id);
        Task<ProductItemDto?> CreateAsync(CreateProductDto dto);
        Task<ProductItemDto?> UpdateAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ChangeStatusAsync(int id);
    }
}
