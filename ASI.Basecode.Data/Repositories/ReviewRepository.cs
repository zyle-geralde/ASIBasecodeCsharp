using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;

        public ReviewRepository(AsiBasecodeDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddReview(Review review)
        {
            await _dbContext.Reviews.AddAsync(review);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Review>> GetAllReviews()
        {
            return await _dbContext.Reviews.ToListAsync();

        }

        public async Task<Review> GetReviewById(string reviewId)
        {
            return await _dbContext.Reviews.SingleAsync(r => r.ReviewId == reviewId);
        }

        public async Task DeleteReview(string reviewId)
        {
            var review = await GetReviewById(reviewId);
            if (review != null)
            {
                _dbContext.Reviews.Remove(review);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Review {reviewId} not found.");
            }
        }
    }
}
