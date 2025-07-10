using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.AccessControl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class PersonProfileController : Controller
    {
        private readonly IPersonProfileService _personProfileService;
        private readonly IReviewService _reviewService;
        private readonly IAccessControlInterface _accessControlInterface;
        private readonly IUserService _userService;

        public PersonProfileController(IPersonProfileService profileService, IReviewService reviewService, IAccessControlInterface accessControlInterface, IUserService userService)
        {
            _personProfileService = profileService;
            _reviewService = reviewService;
            _accessControlInterface = accessControlInterface;
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            //var userid = User.Identity.Name;
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(username))
            {
                return Challenge();
            }
            var profile = await _personProfileService.GetPersonProfile(username);
            if (profile == null)
                return NotFound("Profile not found.");
            var reviews = await _reviewService.GetReviewByUser(username);
            var user = await _userService.GetByEmailForEdit(username);


            var vm = new PersonProfileViewModel
            {
                Id = user.Id,
                UserId = profile.ProfileID,
                FirstName = profile.FirstName,
                MiddleName = profile.MiddleName,
                LastName = profile.LastName,
                AboutMe = profile.AboutMe,
                Birthdate = profile.BirthDate,
                Gender = profile.Gender,
                Location = profile.Location,
                ProfilePicture = profile.ProfilePicture,
                Username = user.UserName,
                Email = user.Email,
                Reviews = reviews
                    .Select(r => new ReviewViewModel
                    {
                        ReviewId = r.ReviewId,
                        BookTitle = r.Book.Title,
                        BookId = r.BookId,
                        UserId = r.UserId,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        Likes = r.Likes,
                        UploadDate = r.UploadDate,
                        UpdatedDate = r.UpdatedDate
                    })
                    .ToList()
            };

            return View(vm);
        }

        [HttpGet]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> Edit(PersonProfileViewModel model)
        {

            //if(!ModelState.IsValid)
            //    return View("~/Views/PersonProfile/Edit.cshtml", model);

            var success = await _personProfileService.EditPersonProfile(model);
            if (!success)
                return NotFound();

            var updatedProfile = await _personProfileService.GetPersonProfile(model.UserId);
            HttpContext.Session.SetString("ProfilePicture", updatedProfile.ProfilePicture ?? "");

            return RedirectToAction("Index", new { success = "personal" });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserInfo(PersonProfileViewModel vm)
        {
            var uvm = new UserViewModel
            {
                Id = vm.Id,
                UserName = vm.Username,
                Email = vm.Email,
            };
            try
            {
                await _userService.UpdateUser(uvm);
                TempData["Success"] = "User info saved!";
            }
            catch (InvalidDataException ex)
            {
                TempData["UserInfoError"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(PersonProfileViewModel model)
        {
   
            var ok = await _userService.ChangePassword(
                   model.Id,
                   model.ChangePassword.CurrentPassword,
                   model.ChangePassword.NewPassword
               );

            if (!ok)
            {
                
                TempData["PwdErrors"] = new[] { "Current password is incorrect." };
            }
            else
            {
                TempData["Success"] = "password";
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ViewUserProfile(int? id)
        {
            string username = null;

            if (id.HasValue)
            {
                var user = await _userService.GetUserById(id.Value);
                if (user == null)
                    return NotFound("User not found.");
                username = user.Email;
            }
            else
            {
                username = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            if (string.IsNullOrEmpty(username))
            {
                return Challenge();
            }

            var profile = await _personProfileService.GetPersonProfile(username);
            if (profile == null)
                return NotFound("Profile not found.");
            var reviews = await _reviewService.GetReviewByUser(username);
            var userVm = await _userService.GetByEmailForEdit(username);

            var vm = new PersonProfileViewModel
            {
                Id = userVm.Id,
                UserId = profile.ProfileID,
                FirstName = profile.FirstName,
                MiddleName = profile.MiddleName,
                LastName = profile.LastName,
                AboutMe = profile.AboutMe,
                Birthdate = profile.BirthDate,
                Gender = profile.Gender,
                Location = profile.Location,
                ProfilePicture = profile.ProfilePicture,
                Username = userVm.UserName,
                Email = userVm.Email,
                Reviews = reviews
                    .Select(r => new ReviewViewModel
                    {
                        ReviewId = r.ReviewId,
                        BookTitle = r.Book.Title,
                        BookId = r.BookId,
                        UserId = r.UserId,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        Likes = r.Likes,
                        UploadDate = r.UploadDate,
                        UpdatedDate = r.UpdatedDate
                    })
                    .ToList()
            };

            return View(vm);
        }

    }
}
