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
    public class FavoriteItemController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFavoriteItemService _favoriteItemService;


        public FavoriteItemController(UserManager<ApplicationUser> userManager, IFavoriteItemService favoriteItemService)
        {
            _userManager = userManager;
            _favoriteItemService = favoriteItemService;
        }

        [HttpGet]
        public async Task<IActionResult> Get( CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            var favoriteItems = await _favoriteItemService.Get(userId, cancellationToken);
            return Ok(favoriteItems);
        }
        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToFavorites(int productId, CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();



            var result = await _favoriteItemService.AddToFavorites(productId, userId, cancellationToken);
            if (!result)
            {
                return BadRequest("Product not found or already exists in favorites");
            }

            return Ok("Added to favorites successfully");
        }
        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Unauthorized();

            var result = await _favoriteItemService.Delete(productId, userId);
            if (!result)
            {
                return NotFound("Favorite item not found");
            }

            return Ok("Removed from favorites successfully");
        }
    }
}
