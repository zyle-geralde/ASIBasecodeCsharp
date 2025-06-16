using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        public async Task<IActionResult> Index()
        {
            var model = await _reviewService.GetAllReviews();
            return View("~/Views/Reviews/Index.cshtml", model);
        }

        public IActionResult Add()
        {
            return View("~/Views/Reviews/Add.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ReviewViewModel reviewModel)
        {
            if (ModelState.IsValid)
            {
                await _reviewService.AddReview(reviewModel);
                return RedirectToAction("Index");
            }
            return View("~/Views/Reviews/Add.cshtml", reviewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            await _reviewService.DeleteReview(id);
            return RedirectToAction("Index");
        }
    }
}
