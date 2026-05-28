using Ecommerce.API.DTOs.Requests.Order;
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
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderService _orderService;

        public OrdersController(UserManager<ApplicationUser> userManager, IOrderService orderService)
        {
            _userManager = userManager;
            _orderService = orderService;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] FilterOrderDto filter,[FromQuery] int pageNumber=1, [FromQuery] int pageSize=10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var userOrders = await _orderService.GetAll(filter, pageNumber, pageSize);

            return Ok(userOrders);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var order = await _orderService.GetByIdAsync(id);
            if (order is null) return NotFound();
            return Ok(order);
        }
        [HttpGet("{id}/Refund")]
        public async Task<IActionResult> Refund(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var result = await _orderService.Refund(id);
            if (!result)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var order = await _orderService.UpdateAsync(id, dto);
            if (order is null) return NotFound();
            return Ok(order);
        }
    }
}
