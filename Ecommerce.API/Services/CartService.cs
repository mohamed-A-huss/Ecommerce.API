using Ecommerce.API.DTOs.Requests.Cart;
using Ecommerce.API.DTOs.Responses.Cart;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Ecommerce.API.Services
{
    public class CartService: ICartService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartRepository _cartRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Promotion> _promotionRepository;
        

        public CartService(UserManager<ApplicationUser> userManager, ICartRepository cartRepository, IRepository<Product> productRepository, IRepository<Promotion> promotionRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _promotionRepository = promotionRepository;
        }
        public async Task<CartResponse> Get(
            string userId,
            string? promotionCode = null,
            CancellationToken cancellationToken = default)
        {
            string message = string.Empty;

            

            var userCart = (await _cartRepository.GetAsync(
                e => e.ApplicationUserId == userId,
                includes: [e => e.Product],
                cancellationToken: cancellationToken))
                .ToList();

            // Convert cart to response items first
            var pricingCart = await ApplyPromotionAsync(userCart, promotionCode);

            return new CartResponse()
            {
                Carts = pricingCart.Select(e => new CartItem
                {
                    ProductId = e.ProductId,
                    Count = e.Count,
                    PricePerProduct = e.FinalPrice,
                    TotalPrice = e.TotalPrice
                }),
                Message = [message]
            };
        }
        public async Task<bool> AddToCart(CartCreateRequest cartCreateRequest,string userId, CancellationToken cancellationToken)
        {
            

            

            var product = await _productRepository.GetOneAsync(e => e.Id == cartCreateRequest.productId, cancellationToken: cancellationToken);
            if (product is null) return false;

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == cartCreateRequest.productId && e.ApplicationUserId == userId);

            if (cart is null)
            {
                await _cartRepository.CreateAsync(new()
                {
                    ApplicationUserId = userId,
                    ProductId = cartCreateRequest.productId,
                    Count = cartCreateRequest.count,
                    PricePerProduct = (double)product.Price,
                    TotalPrice = (double)product.Price * cartCreateRequest.count
                }, cancellationToken: cancellationToken);
            }
            else
            {
                cart.Count += cartCreateRequest.count;
                cart.PricePerProduct = (double)product.Price;
                cart.TotalPrice = (double)product.Price * cartCreateRequest.count;
            }

            await _cartRepository.CommitAsync(cancellationToken);


            return true;
        }
        public async Task<bool> IncrementCount(int productId, string userId)
        {
            

            

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == userId);

            if (cart is null) return false;

            cart.Count += 1;
            await _cartRepository.CommitAsync();

            return true;
        }

        public async Task<bool> DecrementCount(int productId, string userId)
        {
            

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == userId);

            if (cart is null) return false;

            if (cart.Count > 1)
            {
                cart.Count -= 1;
                await _cartRepository.CommitAsync();
            }

            return true;
        }
        public async Task<bool> Delete(int productId, string userId)
        {
            

            

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == userId);

            if (cart is null) return false;

            _cartRepository.Delete(cart);
            await _cartRepository.CommitAsync();

            return true;
        }
        public async Task<List<CartPricingItem>> ApplyPromotionAsync(List<Cart> userCart,string? promotionCode)
        {
            var cartItems = userCart.Select(e => new CartPricingItem
            {
                ProductId = e.ProductId,
                ProductName = e.Product.Name,
                Count = e.Count,

                OriginalPrice = e.PricePerProduct,

                FinalPrice = e.PricePerProduct
            }).ToList();

            if (string.IsNullOrWhiteSpace(promotionCode))
            {
                return cartItems;
            }

            var promotion = await _promotionRepository
                .GetOneAsync(e =>
                    e.Code == promotionCode &&
                    e.Usage > 0);

            if (promotion is null)
            {
                return cartItems;
            }

            if (promotion.ProductId is null)
            {
                foreach (var item in cartItems)
                {
                    item.FinalPrice =
                        item.FinalPrice -
                        (item.FinalPrice * promotion.Discount / 100);
                }
            }
            else
            {
                foreach (var item in cartItems)
                {
                    if (item.ProductId == promotion.ProductId)
                    {
                        item.FinalPrice =
                            item.FinalPrice -
                            (item.FinalPrice * promotion.Discount / 100);
                    }
                }
            }

            return cartItems;
        }

    }
}
