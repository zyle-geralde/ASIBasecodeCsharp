using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.AccessControl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IAccessControlInterface _accessControlInterface;

        public ReviewController(IReviewService reviewService, IAccessControlInterface accessControlInterface)
        {
            _reviewService = reviewService;
            _accessControlInterface = accessControlInterface;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var model = await _reviewService.GetAllReviews();
            return View("~/Views/Reviews/Index.cshtml", model);
        }

        [Authorize]
        public async Task<IActionResult> Add(string? bookId)
        {


            var vm = new ReviewViewModel
            {
                BookId = bookId
            };
            return View("~/Views/Reviews/Add.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Add(ReviewViewModel reviewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get current user ID if not set
                    if (string.IsNullOrEmpty(reviewModel.UserId))
                    {
                        reviewModel.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    }

                    // Set timestamp and initialize likes
                    reviewModel.UploadDate = DateTime.Now;
                    reviewModel.Likes = 0;

                    await _reviewService.AddReview(reviewModel);

                    // Handle AJAX requests
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true });
                    }

                    // Handle normal form submission
                    return RedirectToRoute(
                        routeName: "BookDetails",
                        routeValues: new { bookId = reviewModel.BookId }
                    );
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error adding review: " + ex.Message);
                }
            }

            // Handle AJAX requests with errors
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            // Return to form view with errors
            return View("~/Views/Reviews/Add.cshtml", reviewModel);
        }
        /*public async Task<IActionResult> Add(ReviewViewModel reviewModel)
        {


            if (ModelState.IsValid)
            {
                await _reviewService.AddReview(reviewModel);
                return RedirectToRoute(
                       routeName: "BookDetails",
                       routeValues: new { bookId = reviewModel.BookId }
                   );
            }
            return View("~/Views/Reviews/Add.cshtml", reviewModel);
        }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {

            await _reviewService.DeleteReview(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {


            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var review = await _reviewService.GetReviewById(id);
            if (review == null)
                return NotFound();

            var vm = new ReviewViewModel
            {
                ReviewId = review.ReviewId,
                BookId = review.BookId,
                Rating = review.Rating,
                Comment = review.Comment,
                Likes = review.Likes,
                UserId = review.UserId,
                UploadDate = review.UploadDate,
                UpdatedDate = review.UpdatedDate
            };

            return View("~/Views/Reviews/Edit.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(ReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure user ID is set
                    if (string.IsNullOrEmpty(model.UserId))
                    {
                        model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    }

                    // Set updated timestamp but preserve the original upload date
                    model.UpdatedDate = DateTime.Now;
                    
                    // Make sure not to overwrite the UploadDate if it's already set
                    if (!model.UploadDate.HasValue)
                    {
                        // Fallback if somehow UploadDate is still null
                        var existingReview = await _reviewService.GetReviewById(model.ReviewId);
                        if (existingReview != null && existingReview.UploadDate.HasValue)
                        {
                            model.UploadDate = existingReview.UploadDate;
                        }
                        else
                        {
                            // Last resort fallback - use current date
                            model.UploadDate = DateTime.Now;
                        }
                    }

                    var updated = await _reviewService.UpdateReview(model);
                    if (!updated)
                        return NotFound();

                    // Handle AJAX requests
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true });
                    }

                    // Handle normal form submission
                    return RedirectToRoute(
                        routeName: "BookDetails",
                        routeValues: new { bookId = model.BookId }
                    );
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating review: " + ex.Message);
                }
            }

            // Handle AJAX requests with errors
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            // Return to form view with errors
            return View("~/Views/Reviews/Edit.cshtml", model);
        }
        /* public async Task<IActionResult> Edit(ReviewViewModel model)
         {

             if (!ModelState.IsValid)
                 return View("~/Views/Reviews/Edit.cshtml", model);

             var updated = await _reviewService.UpdateReview(model);
             if (!updated)
                 return NotFound();

             return RedirectToAction("Index");
         }*/


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ReviewByBook(string bookId)
        {
            List<Review> reviews = await _reviewService.GetReviewsByBookId(bookId);
            if (reviews == null || reviews.Count == 0)
            {
                return NotFound("No reviews found for this book.");
            }
            else
            {
                return View("~/Views/Reviews/ReviewByBook.cshtml", reviews.ToList());
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ReviewByUser(string userId=null)
        {
            // in the case that a user might want to see other people's reviews
            if (string.IsNullOrEmpty(userId))
            {
                if (!User.Identity.IsAuthenticated)
                    return Challenge();
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            var reviews = await _reviewService.GetReviewByUser(userId);

                return View("~/Views/Reviews/ReviewByUser.cshtml");
            }
        
    }
}
