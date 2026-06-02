using Ecommerce.API.DTOs.Requests.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [HttpGet("{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviews([FromQuery] FilterReviewDto filter,int productId , [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
                     
            var reviews = await _reviewService.GetAsync(filter, pageNumber, pageSize, productId);
            return Ok(reviews);
        }
        [HttpPost("{productId}")]
        public async Task<IActionResult> CreateReview([FromForm] ReviewCreateRequest reviewCreateRequest, int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var review = await _reviewService.CreateAsync(reviewCreateRequest, userId, productId);
            if (review is null) return BadRequest("Failed to create review");

            return Ok(review);
        }
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var success = await _reviewService.DeleteAsync(reviewId, userId);
            if (!success) return BadRequest("Failed to delete review");

            return Ok();
        }
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromForm] ReviewUpdateRequest reviewUpdateRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return Unauthorized();

            var success = await _reviewService.UpdateAsync(reviewId, userId, reviewUpdateRequest);
            if (success is null) return BadRequest("Failed to update review");

            return Ok(success);
        }
    }
}
