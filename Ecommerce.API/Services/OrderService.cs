using Ecommerce.API.DTOs.Requests.Order;
using Ecommerce.API.DTOs.Responses.Order;
using Ecommerce.API.Repositories;
using Microsoft.AspNetCore.Identity;
using Stripe;

namespace Ecommerce.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Order> _orderRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(UserManager<ApplicationUser> userManager, IRepository<Order> orderRepository, ILogger<OrderService> logger)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
            _logger = logger;
        }
        public async Task<PaginatedOrderResponseDto> GetAll(FilterOrderDto filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetAsync();
            if (orders == null || !orders.Any())
            {
                _logger.LogWarning("No orders found");
                return new PaginatedOrderResponseDto
                {
                    Orders = Enumerable.Empty<OrderItemDto>(),
                    CurrentPage = pageNumber,
                    TotalPages = 0,
                    TotalCount = 0
                };
            }
            if (filter.PaymentMethod.HasValue)
            {
                orders = orders.Where(o => o.PaymentMethod == filter.PaymentMethod.Value);
            }
            if (filter.PaymentStatus.HasValue)
            {
                orders = orders.Where(o => o.PaymentStatus == filter.PaymentStatus.Value);
            }
            if (filter.OrderStatus.HasValue)
            {
                orders = orders.Where(o => o.OrderStatus == filter.OrderStatus.Value);
            }
            var totalCount = await orders.CountAsync();

            double totalPages = Math.Ceiling(totalCount / (double)pageSize);

            orders = orders.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            var result = await orders.ToListAsync(cancellationToken);
            return new PaginatedOrderResponseDto
            {
                Orders = result.Select(o => new OrderItemDto
                {
                    Id = o.Id,
                    TotalPrice = o.TotalPrice,
                    PaymentMethod = o.PaymentMethod,
                    PaymentStatus = o.PaymentStatus,
                    OrderStatus = o.OrderStatus,
                    Tracker = o.Tracker,
                    ShippedDate = o.ShippedDate,
                    CreateAt = o.CreateAt
                }),
                CurrentPage = pageNumber,
                TotalPages = (int)totalPages,
                TotalCount = totalCount
            };
        }

        public async Task<OrderItemDto?> GetByIdAsync(int id)
        {

            var order = await _orderRepository.GetOneAsync(c => c.Id == id);
            if (order is null)
            {
                _logger.LogWarning("Order with id {id} was not found", id);
                return null;
            }

            _logger.LogInformation("Order with id {id} was found", id);
            OrderItemDto response = new()
            {
                Id = order.Id,
                TotalPrice = order.TotalPrice,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                OrderStatus = order.OrderStatus,
                Tracker = order.Tracker,
                ShippedDate = order.ShippedDate,
                CreateAt = order.CreateAt
            };
            return response;
        }
        public async Task<bool> Refund(int id)
        {
            var order = await _orderRepository.GetOneAsync(c => c.Id == id);
            if (order is null)
            {
                _logger.LogWarning("Order with id {id} was not found", id);
                return false;
            }
            if (order.PaymentStatus == PaymentStatus.Refunded ||order.PaymentStatus != PaymentStatus.Successed)
            {
                _logger.LogWarning("Order with id {id} cannot be refunded because its payment status is not successed or already refunded", id);
                return false;
            }


            var options = new RefundCreateOptions()
            {
                Reason = RefundReasons.Unknown,
                Amount = ((long)order.TotalPrice * 100) - (5 * 100),
                PaymentIntent = order.TransactionId
            };
            var service = new RefundService();
            var session = service.Create(options);

            order.OrderStatus = OrderStatus.Canceled;
            order.PaymentStatus = PaymentStatus.Refunded;

            _orderRepository.Update(order);
            var result = await _orderRepository.CommitAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to refund order with id {id}", id);
                return false;
            }
            return true;
        }

        public async Task<OrderItemDto?> UpdateAsync(int id, UpdateOrderDto dto)
        {
            var order = await _orderRepository.GetOneAsync(c => c.Id == id);
            if (order is null)
            {
                _logger.LogWarning("Order with id {id} was not found", id);
                return null;
            }

            // Update the order properties with the values from the DTO
            if (dto.PaymentMethod.HasValue)
            {
                order.PaymentMethod = dto.PaymentMethod.Value;
            }
            if (dto.PaymentStatus.HasValue)
            {
                order.PaymentStatus = dto.PaymentStatus.Value;
            }
            if (dto.OrderStatus.HasValue)
            {
                order.OrderStatus = dto.OrderStatus.Value;
            }
            if (dto.ShippedDate.HasValue)
            {
                order.ShippedDate = dto.ShippedDate.Value;
            }

            _orderRepository.Update(order);
            var result = await _orderRepository.CommitAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to update order with id {id}", id);
                return null;
            }

            return new OrderItemDto
            {
                Id = order.Id,
                TotalPrice = order.TotalPrice,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                OrderStatus = order.OrderStatus,
                Tracker = order.Tracker,
                ShippedDate = order.ShippedDate,
                CreateAt = order.CreateAt
            };
        }
    }
}

