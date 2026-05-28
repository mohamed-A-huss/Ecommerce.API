using Ecommerce.API.DTOs.Requests.Cart;
using Ecommerce.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe.Climate;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IRepository<Cart> _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Models.Order> _orderRepository;
        private readonly IRepository<Models.OrderItem> _orderItemRepository;
        public CartsController(ICartService cartService, UserManager<ApplicationUser> userManager, IRepository<Cart> cartRepository, IRepository<Models.Order> orderRepository, IRepository<Models.OrderItem> orderItemRepository)
        {
            _cartService = cartService;
            _userManager = userManager;
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Get(string? promotionCode = null, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var cart = await _cartService.Get(userId, promotionCode, cancellationToken);
            return Ok(cart);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(CartCreateRequest cartCreateRequest, CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();



            var result = await _cartService.AddToCart(cartCreateRequest, userId, cancellationToken);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpPut("{productId}/increment")]
        public async Task<IActionResult> IncrementCount(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var result = await _cartService.IncrementCount(productId, userId);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpPut("{productId}/decrement")]
        public async Task<IActionResult> DecrementCount(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();
            var result = await _cartService.DecrementCount(productId, userId);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var result = await _cartService.Delete(productId, userId);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpGet("Pay")]
        public async Task<IActionResult> Pay(string? promotionCode = null)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return Unauthorized();

            var userCart = await _cartRepository
                .GetAsync(e => e.ApplicationUserId == user.Id,
                includes: [e => e.Product]);

            var pricingCart = await _cartService
                .ApplyPromotionAsync(userCart.ToList(), promotionCode);

            Models.Order order = new()
            {
                ApplicationUserId = user.Id,
                TotalPrice = pricingCart.Sum(e => e.TotalPrice)
            };
            await _orderRepository.CreateAsync(order);
            await _orderRepository.CommitAsync();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/checkout/success?orderId={order.Id}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/cancel?orderId={order.Id}",
            };

            foreach (var item in pricingCart)
            {
                options.LineItems.Add(
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName,
                                Description = item.ProductName,
                            },
                            UnitAmount = (long)item.FinalPrice * 100,
                        },
                        Quantity = item.Count,
                    });
            }


            var service = new SessionService();
            var session = service.Create(options);
            order.SessionId = session.Id;
            await _orderRepository.CommitAsync();

            return Ok(new APIResponse()
            {
                StatusCode = 200,
                Message = [session.Url]
            });
        }

    }
}
