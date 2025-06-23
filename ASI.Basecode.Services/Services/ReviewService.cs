using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;

        }

        public async Task AddReview(ReviewViewModel reviewModel)
        {
            if (reviewModel == null)
            {
                throw new ArgumentNullException(nameof(reviewModel), "Review cannot be null");
            }

            var review = new Review
            {
                ReviewId = Guid.NewGuid().ToString(),
                BookId = reviewModel.BookId,
                Rating = reviewModel.Rating,
                Comment = reviewModel.Comment,
                Likes = reviewModel.Likes,
                UserId = reviewModel.UserId,
                UploadDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

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

        public async Task<bool> UpdateReview(ReviewViewModel reviewModel)
        {
            if (reviewModel == null)
                throw new ArgumentNullException(nameof(reviewModel));

            var existing = await _reviewRepository.GetReviewById(reviewModel.ReviewId);
            if (existing == null)
                return false;

            _mapper.Map(reviewModel, existing);

            //existing.Rating = reviewModel.Rating;
            //existing.Comment = reviewModel.Comment;
            //existing.Likes = reviewModel.Likes;
            //existing.ReviewImage = reviewModel.ReviewImage;
            existing.UpdatedDate = DateTime.UtcNow;

            await _reviewRepository.UpdateReview(existing);
            return true;
        }

        public async Task<List<Review>> GetReviewsByBookId(string bookId)
        {
            if (string.IsNullOrEmpty(bookId))
            {
                throw new ArgumentException("No book id provided", nameof(bookId));
            }
            var reviews = await _reviewRepository.GetReviewsByBookId(bookId);
            return reviews.ToList();
        }

        public async Task<List<Review>> GetReviewByUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("No user ID provided");
            }
            var reviews = await _reviewRepository.GetReviewByUser(userId);
            return reviews.ToList();
        }
    }
}
