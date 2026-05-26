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

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return new CartResponse()
                {
                    Carts = null,
                    Message = ["User not found"]
                };
            }

            var userCart = (await _cartRepository.GetAsync(
                e => e.ApplicationUserId == user.Id,
                includes: [e => e.Product],
                cancellationToken: cancellationToken))
                .ToList();

            // Convert cart to response items first
            var cartItems = userCart.Select(e => new CartItem
            {
                ProductId = e.ProductId,
                Count = e.Count,
                PricePerProduct = e.PricePerProduct,
                TotalPrice = e.PricePerProduct * e.Count
            }).ToList();

            // Apply promotion only in response
            if (!string.IsNullOrWhiteSpace(promotionCode))
            {
                var promotion = await _promotionRepository.GetOneAsync(
                    e => e.Code == promotionCode && e.Usage > 0);

                if (promotion is null)
                {
                    return new CartResponse()
                    {
                        Carts = cartItems,
                        Message = ["Invalid promotion"]
                    };
                }

                // Promotion on whole cart
                if (promotion.ProductId is null)
                {
                    foreach (var item in cartItems)
                    {
                        var discountedPrice =
                            item.PricePerProduct -
                            (item.PricePerProduct * promotion.Discount / 100);

                        item.PricePerProduct = discountedPrice;
                        item.TotalPrice = discountedPrice * item.Count;
                    }

                    message = "Promotion applied successfully";
                }
                else
                {
                    bool applied = false;

                    foreach (var item in cartItems)
                    {
                        if (item.ProductId == promotion.ProductId)
                        {
                            var discountedPrice =
                                item.PricePerProduct -
                                (item.PricePerProduct * promotion.Discount / 100);

                            item.PricePerProduct = discountedPrice;
                            item.TotalPrice = discountedPrice * item.Count;

                            applied = true;
                        }
                    }

                    message = applied
                        ? "Promotion applied successfully"
                        : "Can not apply this promotion code on the current cart";
                }
            }

            return new CartResponse()
            {
                Carts = cartItems,
                Message = [message]
            };
        }
        public async Task<bool> AddToCart(CartCreateRequest cartCreateRequest,string userId, CancellationToken cancellationToken)
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (userId is null) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            var product = await _productRepository.GetOneAsync(e => e.Id == cartCreateRequest.productId, cancellationToken: cancellationToken);
            if (product is null) return false;

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == cartCreateRequest.productId && e.ApplicationUserId == user.Id);

            if (cart is null)
            {
                await _cartRepository.CreateAsync(new()
                {
                    ApplicationUserId = user.Id,
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
            

            var user = await _userManager.FindByIdAsync( userId);
            if (user is null) return false;

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == user.Id);

            if (cart is null) return false;

            cart.Count += 1;
            await _cartRepository.CommitAsync();

            return true;
        }

        public async Task<bool> DecrementCount(int productId, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == user.Id);

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
            

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == user.Id);

            if (cart is null) return false;

            _cartRepository.Delete(cart);
            await _cartRepository.CommitAsync();

            return true;
        }

    }
}
