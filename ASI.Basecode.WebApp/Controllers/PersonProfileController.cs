using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class PersonProfileController : Controller
    {
        private readonly IPersonProfileService _personProfileService;
        private readonly IReviewService _reviewService;

        public PersonProfileController(IPersonProfileService profileService, IReviewService reviewService)
        {
            _personProfileService = profileService;
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(username))
            {
                return Challenge();
            }
            var profile = await _personProfileService.GetPersonProfile(username);
            if (profile == null)
                return NotFound("Profile not found.");
            var reviews = await _reviewService.GetReviewByUser(username);

            var vm = new PersonProfileViewModel
            {
                UserId = profile.ProfileID,
                FirstName = profile.FirstName,
                MiddleName = profile.MiddleName,
                LastName = profile.LastName,
                AboutMe = profile.AboutMe,
                Birthdate = profile.BirthDate,
                Gender = profile.Gender,
                Location = profile.Location,
                ProfilePicture = profile.ProfilePicture,
                Reviews = reviews
                    .Select(r => new ReviewViewModel
                    {
                        ReviewId = r.ReviewId,
                        BookId = r.BookId,
                        UserId = r.UserId,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        Likes = r.Likes,
                        ReviewImage = r.ReviewImage,
                        UploadDate = r.UploadDate,
                        UpdatedDate = r.UpdatedDate
                    })
                    .ToList()
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(username))
            {
                return Challenge();
            }
            var profile = await _personProfileService.GetPersonProfile(username);
            if (profile == null)
                return NotFound("Profile not found.");

            var vm = new PersonProfileViewModel
            {
                UserId = profile.ProfileID,
                FirstName = profile.FirstName,
                MiddleName = profile.MiddleName,
                LastName = profile.LastName,
                AboutMe = profile.AboutMe,
                Birthdate = profile.BirthDate,
                Gender = profile.Gender,
                Location = profile.Location,
                ProfilePicture = profile.ProfilePicture
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PersonProfileViewModel model)
        {
            //if(!ModelState.IsValid)
            //    return View("~/Views/PersonProfile/Edit.cshtml", model);

            var updated = await _personProfileService.EditPersonProfile(model);
            if (!updated)
                return NotFound();

            return RedirectToAction("Index");

        }

    }
}
