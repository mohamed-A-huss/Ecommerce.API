
using Ecommerce.API.DTOs.Responses.FavorateItem;

namespace Ecommerce.API.Services
{
    public class FavoriteItemService : IFavoriteItemService
    {
        private readonly IRepository<FavoriteItem> _FavoriteItemRepository;
        private readonly IRepository<Product> _productRepository;


        public FavoriteItemService( IRepository<Product> productRepository, IRepository<FavoriteItem> favoriteItemRepository)
        {
            _productRepository = productRepository;
            _FavoriteItemRepository = favoriteItemRepository;
        }

        public async Task<FavoriteItemResponse> Get(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var userFavorites = (await _FavoriteItemRepository.GetAsync(
                e => e.ApplicationUserId == userId,
                includes: [e => e.Product],
                cancellationToken: cancellationToken))
                .ToList();

            // Convert cart to response items first

            return new FavoriteItemResponse()
            {
                FavoriteItems = userFavorites.Select(e => new FavItem
                {
                    ProductId = e.ProductId,
                    ProductName = e.Product.Name,
                    ImageUrl = e.Product.MainImg,
                    Price = (double)e.Product.Price
                }),
                Message = ["Success"]
            };
        }
        public async Task<bool> AddToFavorites(int productId, string userId, CancellationToken cancellationToken)
        {

            var product = await _productRepository.GetOneAsync(e => e.Id == productId, cancellationToken: cancellationToken);
            if (product is null) return false;

            var favoriteItem = await _FavoriteItemRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == userId);

            if (favoriteItem is null)
            {
                await _FavoriteItemRepository.CreateAsync(new()
                {
                    ApplicationUserId = userId,
                    ProductId = productId,
                }, cancellationToken: cancellationToken);
            }
            else
            {
                return false;
            }

            await _FavoriteItemRepository.CommitAsync(cancellationToken);


            return true;
        }
        public async Task<bool> Delete(int productId, string userId)
        {




            var favoriteItem = await _FavoriteItemRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == userId);

            if (favoriteItem is null) return false;

            _FavoriteItemRepository.Delete(favoriteItem);
            await _FavoriteItemRepository.CommitAsync();

            return true;
        }
    }
}
