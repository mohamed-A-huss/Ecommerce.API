namespace Ecommerce.API.DTOs.Responses.Cart
{
    public class CartResponse
    {
        public IEnumerable<CartItem>? Carts { get; set; } 

        

        public string[]? Message { get; set; } = null!;
    }
}
