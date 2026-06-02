namespace Ecommerce.API.DTOs.Requests.Review
{
    public class ReviewCreateRequest
    {

        public string Comment { get; set; } = string.Empty;
        [Range(1, 5)]
        public int Rate { get; set; }
        public IFormFile? Img { get; set; } 
    }
}
