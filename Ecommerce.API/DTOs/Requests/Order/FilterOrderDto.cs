namespace Ecommerce.API.DTOs.Requests.Order
{
    public class FilterOrderDto
    {
        public PaymentMethod? PaymentMethod { get; set; } 
        public PaymentStatus? PaymentStatus { get; set; }
        public OrderStatus? OrderStatus { get; set; }
    }
}
