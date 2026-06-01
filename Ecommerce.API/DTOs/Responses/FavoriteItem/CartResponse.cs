namespace Ecommerce.API.DTOs.Responses.FavorateItem

{
    public class FavoriteItemResponse
    {
        public IEnumerable<FavItem>? FavoriteItems { get; set; } 
        public string[]? Message { get; set; } = null!;
    }
}
