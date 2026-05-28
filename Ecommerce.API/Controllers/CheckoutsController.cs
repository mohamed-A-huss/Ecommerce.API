using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class CheckoutsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly ILogger<CheckoutsController> _logger;
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;


        public CheckoutsController(IRepository<Order> orderRepository,
            AppDbContext context,
            ILogger<CheckoutsController> logger,
            ICartRepository cartRepository,
            IRepository<OrderItem> orderItemRepository,
            UserManager<ApplicationUser> userManager)
        {
            _orderRepository = orderRepository;
            _context = context;
            _logger = logger;
            _cartRepository = cartRepository;
            _orderItemRepository = orderItemRepository;
            _userManager = userManager;
        }

        [HttpGet("{orderId}/Success")]
        public async Task<IActionResult> Success(int orderId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();
            var transaction = _context.Database.BeginTransaction();

            try
            {
                var order = await _orderRepository.GetOneAsync(e => e.Id == orderId,
                    includes: [e => e.ApplicationUser]);

                if (order is null) return NotFound();

                // Update Payment Status
                //if(order.PaymentMethod == PaymentMethod.Visa)
                //    order.PaymentStatus = PaymentStatus.Successed;
                //else
                //    order.PaymentStatus = PaymentStatus.COD;

                order.PaymentStatus = order.PaymentMethod == PaymentMethod.Visa ? PaymentStatus.Successed : PaymentStatus.COD;

                // Update Order Status
                order.OrderStatus = OrderStatus.InProcessing;

                // Update Transaction Id
                var service = new SessionService();
                var session = service.Get(order.SessionId);
                order.TransactionId = session.PaymentIntentId;

                // Move Cart => Order Item
                var userCart = await _cartRepository
                .GetAsync(e => e.ApplicationUserId == order.ApplicationUserId,
                includes: [e => e.Product]);

                foreach (var item in userCart)
                {
                    await _orderItemRepository.CreateAsync(new()
                    {
                        OrderId = orderId,
                        ProductId = item.ProductId,
                        Count = item.Count,
                        PricePerProduct = item.PricePerProduct,
                    });
                }

                // Delete Old Cart
                _cartRepository.DeleteRange(userCart);

                // TODO: Send Mail

                await _orderRepository.CommitAsync();
                //await _cartRepository.CommitAsync(); 
                //await _orderItemRepository.CommitAsync(); 
                transaction.Commit();

                return Ok(new PaymentSuccessResponse()
                {
                    OrderDate = order.CreateAt,
                    PaymentMethod = order.PaymentMethod.ToString(),
                    TotalPrice = order.TotalPrice,
                    OrderId = order.Id,
                    Email = order.ApplicationUser.Email!
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                transaction.Rollback();
                return BadRequest();
            }
        }
    }
}
