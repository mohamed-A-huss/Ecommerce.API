using Ecommerce.API.DTOs.Requests.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        public CartsController(ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Get(string? promotionCode = null, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return NotFound();

            
            var cart = await _cartService.Get(userId, promotionCode, cancellationToken);
            return Ok(cart);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(CartCreateRequest cartCreateRequest, CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return NotFound();

            

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
            if (userId is null) return NotFound();

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
            if (userId is null) return NotFound();
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
            if (userId is null) return NotFound();
            var result = await _cartService.Delete(productId, userId);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }

    }
}
