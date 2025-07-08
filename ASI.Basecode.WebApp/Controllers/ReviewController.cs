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
                BookId = bookId,
                UserId = GetCurrentUserId()
            };
            return View("~/Views/Reviews/Add.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Add(ReviewViewModel reviewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Reviews/Add.cshtml", reviewModel);
            }

                try
                {
                    reviewModel.UserId = GetCurrentUserId();

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

            return View("~/Views/Reviews/Add.cshtml", reviewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var deleted = await _reviewService.DeleteReview(id);
                if (!deleted)
                {
                    TempData["Error"] = "Review not found or could not be deleted.";
                }
                else
                {
                    TempData["Success"] = "Review deleted successfully.";
                }

            }
            catch (Exception e)
            {
                TempData["Error"] = "Error deleting review. Please try again";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {


            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Review ID is required");

            }

            var review = await _reviewService.GetReviewById(id);
            if (review == null)
            {
                return NotFound("Review not found");

            }

            return View("~/Views/Reviews/Edit.cshtml", review);
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
                    model.UserId = GetCurrentUserId();
                    model.UpdatedDate = DateTime.Now;
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

            return View("~/Views/Reviews/ReviewByUser.cshtml", reviews);
            }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

    }
}
