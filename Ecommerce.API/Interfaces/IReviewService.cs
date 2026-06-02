using Ecommerce.API.DTOs.Requests.Review;
using Ecommerce.API.DTOs.Responses.Review;

namespace Ecommerce.API.Interfaces
{
    public interface IReviewService
    {
        Task<PaginatedReviewResponse> GetAsync(FilterReviewDto filter, int pageNumber, int pageSize, int productId);
        Task<ReviewItem?> CreateAsync([FromForm] ReviewCreateRequest reviewCreateRequest, string UserId, int ProductId);
        Task<ReviewItem?> UpdateAsync(int ReviewId, string UserId, ReviewUpdateRequest reviewUpdateRequest);
        Task<bool> DeleteAsync(int id, string UserId);
    }
}
