namespace Ecommerce.API.DTOs.Requests.Review
{
    public class ReviewUpdateRequest
    {
        public string? Comment { get; set; } = string.Empty;
        [Range(1, 5)]
        public int? Rate { get; set; }
        public IFormFile? Img { get; set; }
    }
}
