using Ecommerce.API.DTOs.Requests.Cart;
using Ecommerce.API.DTOs.Responses.Cart;

namespace Ecommerce.API.Interfaces
{
    public interface ICartService
    {
        Task<CartResponse> Get(string userId, string? promotionCode = null, CancellationToken cancellationToken = default);
        Task<bool> AddToCart(CartCreateRequest cartCreateRequest, string userId, CancellationToken cancellationToken);

         Task<bool> IncrementCount(int productId, string userId);

         Task<bool> DecrementCount(int productId, string userId);
        Task<bool> Delete(int productId, string userId);


    }
}
