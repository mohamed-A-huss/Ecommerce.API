namespace Ecommerce.API.DTOs.Requests.Order
{
    public class UpdateOrderDto
    {
        public PaymentMethod? PaymentMethod { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public OrderStatus? OrderStatus { get; set; }
        public DateTime? ShippedDate { get; set; }
    }
}
