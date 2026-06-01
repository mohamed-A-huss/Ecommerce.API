namespace Ecommerce.API.DTOs.Responses.FavorateItem

{
    public class FavItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public double Price { get; set; }
    }
}
