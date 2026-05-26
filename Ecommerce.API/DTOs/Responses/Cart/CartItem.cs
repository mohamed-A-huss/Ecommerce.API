namespace Ecommerce.API.DTOs.Responses.Cart
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public double PricePerProduct { get; set; }
        public double TotalPrice { get; set; }
    }
}
