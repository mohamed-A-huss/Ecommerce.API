using Ecommerce.API.DTOs.Requests.Promotion;
using Ecommerce.API.DTOs.Responses.Promotion;
using Ecommerce.API.Services;
using Ecommerce.API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PromotionsController(IPromotionService promotionService, UserManager<ApplicationUser> userManager)
        {
            _promotionService = promotionService;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] FilterPromotionDto filter,[FromQuery] int pageNumber=1, [FromQuery] int pageSize=10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();
            var result = await _promotionService.GetAll(filter, pageNumber, pageSize);

            return Ok(new PaginatedPromotionResponseDto
            {
                Promotions = result.Promotions,

                TotalPages = result.TotalPages,
                CurrentPage = result.CurrentPage,
                Query = result.Query
            });


        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var result = await _promotionService.GetByIdAsync(id);
            if (result is null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreatePromotionDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var result = await _promotionService.CreateAsync(dto);
            if (result is null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdatePromotionDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var result = await _promotionService.UpdateAsync(id, dto);
            if (result is null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var result = await _promotionService.DeleteAsync(id);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }

    }
}