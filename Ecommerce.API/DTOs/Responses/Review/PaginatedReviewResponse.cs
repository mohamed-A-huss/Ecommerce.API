namespace Ecommerce.API.DTOs.Responses.Review
{
    public class PaginatedReviewResponse
    {
        public IEnumerable<ReviewItem> Reviews { get; set; } = [];

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }
    }
}
