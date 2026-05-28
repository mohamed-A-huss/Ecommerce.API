namespace Ecommerce.API.DTOs.Responses.Order
{
    public class OrderItemDto
    {
        public int Id { get; set; }

        public DateTime CreateAt { get; set; }

        public PaymentMethod PaymentMethod { get; set; } 
        public PaymentStatus PaymentStatus { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string? Tracker { get; set; }
        public DateTime? ShippedDate { get; set; }
        public double TotalPrice { get; set; }
    }
}
