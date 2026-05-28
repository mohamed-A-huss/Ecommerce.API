namespace Ecommerce.API.DTOs.Responses.Cart
{
    public class CartPricingItem
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Count { get; set; }

        public double OriginalPrice { get; set; }

        public double FinalPrice { get; set; }

        public double TotalPrice => FinalPrice * Count;
    }
}
