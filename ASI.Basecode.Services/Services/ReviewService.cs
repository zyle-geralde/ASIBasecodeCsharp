using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task AddReview(Review review)
        {
            if (review == null)
            {
                throw new ArgumentNullException(nameof(review), "Review cannot be null");
            }
            await _reviewRepository.AddReview(review);
        }

        public async Task<List<Review>> GetAllReviews()
        {
            return await _reviewRepository.GetAllReviews();

        }

        public async Task<Review> GetReviewById(string reviewId)
        {
            if (string.IsNullOrEmpty(reviewId))
            {
                throw new ArgumentException("No review id provided", nameof(reviewId));
            }
            return await _reviewRepository.GetReviewById(reviewId);
        }

        public async Task<bool> DeleteReview(string reviewId)
        {
            if (string.IsNullOrEmpty(reviewId))
            {
                throw new ArgumentException("No review id provided", nameof(reviewId));
            }

            var existingReview = await _reviewRepository.GetReviewById(reviewId);
            if (existingReview == null)
            {
                return false;
            }
            
            await _reviewRepository.DeleteReview(reviewId);
            return true;


        }
    }
}
