using Ecommerce.API.DTOs.Responses.FavorateItem;

namespace Ecommerce.API.Interfaces
{
    public interface IFavoriteItemService
    {
        Task<FavoriteItemResponse> Get(string userId,CancellationToken cancellationToken = default);
        Task<bool> AddToFavorites(int productId, string userId, CancellationToken cancellationToken);
        Task<bool> Delete(int productId, string userId);
    }
}
