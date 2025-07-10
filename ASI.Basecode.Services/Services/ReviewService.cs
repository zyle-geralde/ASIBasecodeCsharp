using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IBookRepository bookRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;

        }

        public async Task AddReview(ReviewViewModel reviewModel)
        {
            if (reviewModel == null)
            {
                throw new ArgumentNullException(nameof(reviewModel), "Review cannot be null");
            }
            try
            {
                var review = _mapper.Map<Review>(reviewModel);
                review.ReviewId = Guid.NewGuid().ToString();
                review.UploadDate = DateTime.Now;
                review.UpdatedDate = DateTime.Now;
                await _reviewRepository.AddReview(review);
                await _bookRepository.GetReviewCount(review.BookId);
                await _bookRepository.calculateAverageRating(review.BookId);
            }

            catch(Exception e)
            {
                throw;
            }
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

            try
            {
                var existingReview = await _reviewRepository.GetReviewById(reviewId);
                if (existingReview == null)
                {
                    return false;
                }
                await _reviewRepository.DeleteReview(reviewId);
                await _bookRepository.GetReviewCount(existingReview.BookId);
                await _bookRepository.calculateAverageRating(existingReview.BookId);
                return true;

            }
            catch(Exception e)
            {
                throw;
            }
        }

        public async Task<bool> UpdateReview(ReviewViewModel reviewModel)
        {
            if (reviewModel == null)
                throw new ArgumentNullException(nameof(reviewModel));
            try
            {
                var existing = await _reviewRepository.GetReviewById(reviewModel.ReviewId);
                if (existing == null)
                {
                    return false;
                }

                _mapper.Map(reviewModel, existing);

                existing.UpdatedDate = DateTime.Now;

                await _reviewRepository.UpdateReview(existing);
                await _bookRepository.GetReviewCount(reviewModel.BookId);
                await _bookRepository.calculateAverageRating(reviewModel.BookId);

                return true;

            }
            catch(Exception e)
            {
                throw;
            }
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
