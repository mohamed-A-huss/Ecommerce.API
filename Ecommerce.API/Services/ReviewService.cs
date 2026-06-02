using Ecommerce.API.DTOs.Requests.Review;
using Ecommerce.API.DTOs.Responses.Review;

namespace Ecommerce.API.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IImageService _imageService;
        private readonly IRepository<UserReview> _userReview;
        private readonly ILogger<ReviewService> _logger;
        private readonly IRepository<OrderItem> _orderItemRepository;

        public ReviewService(IImageService imageService, IRepository<UserReview> userReview, ILogger<ReviewService> logger, IRepository<OrderItem> orderItemRepository)
        {
            _imageService = imageService;
            _userReview = userReview;
            _logger = logger;
            _orderItemRepository = orderItemRepository;
        }
        public async Task<PaginatedReviewResponse> GetAsync(FilterReviewDto filter, int pageNumber, int pageSize, int productId)
        {
            var reviews = await _userReview.GetAsync(e => e.ProductId == productId, tracked: false, includes:[ r => r.ApplicationUser]);
            if (reviews.Count() == 0)
            {
                _logger.LogInformation("No products found");
                return new PaginatedReviewResponse
                {
                    Reviews = Enumerable.Empty<ReviewItem>(),
                    CurrentPage = pageNumber,
                    TotalPages = 0,
                    TotalCount = 0
                };

            }
            if (filter.RateAscending)
            {
                reviews = reviews.OrderBy(r => r.Rate);
            }
            else if (filter.DateAscending)
            {
                reviews = reviews.OrderBy(r => r.CreateAt);
            }



            var totalCount = await reviews.CountAsync();

            double totalPages = Math.Ceiling(totalCount / (double)pageSize);

            reviews = reviews.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            var result = await reviews.ToListAsync();
            return new PaginatedReviewResponse
            {
                Reviews = result.Select(r => new ReviewItem
                {
                    Id = r.Id,
                    Comment = r.Comment,
                    Rate = r.Rate,
                    CreateAt = r.CreateAt,
                    Img = r.Img,
                    ReviewerName = r.ApplicationUser.FirstName + " " + r.ApplicationUser.LastName

                }),
                CurrentPage = pageNumber,
                TotalPages = (int)totalPages,
                TotalCount = totalCount
            };
        }
        public async Task<ReviewItem?> CreateAsync([FromForm] ReviewCreateRequest reviewCreateRequest, string UserId, int ProductId)
        { var hasPurchased = await _orderItemRepository.
                GetAsync(oi => oi.ProductId == ProductId 
                && oi.Order.ApplicationUserId == UserId 
                && oi.Order.OrderStatus == OrderStatus.Completed);
            if (!hasPurchased.Any())
            {
                _logger.LogWarning("User with id {userId} has not purchased product with id {productId} and cannot leave a review", UserId, ProductId);
                return null;
            }
            var existingReview = await _userReview.GetOneAsync(e => e.ProductId == ProductId &&e.ApplicationUserId == UserId);
            if (existingReview is not null)
            {
                _logger.LogInformation("User has already reviewed this product");

                return null;
            }
            try
                {
                    string imageName = null;
                    if (reviewCreateRequest.Img is not null)
                    {
                     imageName = await _imageService.SaveImageAsync(reviewCreateRequest.Img, "Reviews");                        
                    }
                    var newReview = new UserReview
                    {
                        ProductId = ProductId,
                        ApplicationUserId = UserId,
                        Comment = reviewCreateRequest.Comment,
                        Rate = reviewCreateRequest.Rate,
                        Img = imageName
                    };

                    await _userReview.CreateAsync(newReview);
                    var result = await _userReview.CommitAsync();
                    if (result <= 0)
                    {
                        _logger.LogError("Failed to create review");
                        return null;
                    }
                    _logger.LogInformation("Review with id {id} was created", newReview.Id);
                    return new ReviewItem
                    {
                        Id = newReview.Id,
                        Comment = newReview.Comment,
                        Rate = newReview.Rate,
                        CreateAt = newReview.CreateAt,
                        Img = newReview.Img,
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while creating the review");
                    return null;
                }
            
        }

        public async Task<bool> DeleteAsync(int id, string UserId)
        {
            var review = await _userReview.GetOneAsync(p => p.Id == id && p.ApplicationUserId == UserId);
            if (review is null)
            {
                _logger.LogWarning("Review with id {id} was not found", id);
                return false;
            }
            if (review.Img is not null)
            {
                await _imageService.DeleteImageAsync(review.Img, "Reviews");
            }
            _userReview.Delete(review);
            await _userReview.CommitAsync();
            _logger.LogInformation("Review with id {id} was deleted", id);
            return true;
        }


        public async Task<ReviewItem?> UpdateAsync(int ReviewId,string UserId, ReviewUpdateRequest reviewUpdateRequest)
        {
            var review = await _userReview.GetOneAsync(p => p.Id == ReviewId && p.ApplicationUserId == UserId);
            
            if (review is null)
            {
                _logger.LogWarning("Review with id {id} was not found", ReviewId);
                return null;
            } 
            try {
                if (reviewUpdateRequest.Img is not null)
                {
                    if (review.Img is not null)
                        await _imageService.DeleteImageAsync(review.Img, "Reviews");
                    var imageName = await _imageService.SaveImageAsync(reviewUpdateRequest.Img, "Reviews");
                    review.Img = imageName;
                }
                review.Comment = reviewUpdateRequest.Comment ?? review.Comment;
                review.Rate = reviewUpdateRequest.Rate ?? review.Rate;
                _userReview.Update(review);
                await _userReview.CommitAsync();

                return new ReviewItem
                {
                    Id = review.Id,
                    Comment = review.Comment,
                    Rate = review.Rate,
                    CreateAt = review.CreateAt,
                    Img = review.Img,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the review with id {id}", ReviewId);
                return null;
            }
            
        }
    }
}
