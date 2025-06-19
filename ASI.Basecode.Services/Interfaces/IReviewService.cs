using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IReviewService
    {
        Task AddReview(ReviewViewModel reviewModel);
        Task<List<Review>> GetAllReviews();
        Task<Review> GetReviewById(string reviewId);
        Task<bool> DeleteReview(string reviewId);
        Task<bool> UpdateReview(ReviewViewModel reviewModel);
        Task<List<Review>> GetReviewsByBookId(string bookId);
    }
}
