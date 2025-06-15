using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IReviewService
    {
        Task AddReview(Review review);
        Task<List<Review>> GetAllReviews();
    }
}
