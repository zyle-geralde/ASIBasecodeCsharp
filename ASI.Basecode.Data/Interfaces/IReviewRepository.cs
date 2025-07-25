﻿using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IReviewRepository
    {
        Task AddReview(Review review);
        Task<List<Review>> GetAllReviews();
        Task<Review> GetReviewById(string reviewId);
        Task DeleteReview(string reviewId);
        Task UpdateReview(Review review);
        Task<List<Review>> GetReviewsByBookId(string bookId);
        Task<List<Review>> GetReviewByUser(string userId);
    }
}
