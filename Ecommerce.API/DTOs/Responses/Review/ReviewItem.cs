namespace Ecommerce.API.DTOs.Responses.Review
{
    public class ReviewItem
    {
        public int Id { get; set; }

        public string Comment { get; set; } = string.Empty;
        public int Rate { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; } 
        public string? Img { get; set; }
    }
}
