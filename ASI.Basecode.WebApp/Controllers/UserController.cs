using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm, string sortOrder, int? page)
        {
            const int PageSize = 10;

            int pageIndex = page.GetValueOrDefault(1);
            ViewData["CurrentSearch"] = searchTerm;
            ViewData["CurrentSort"] = sortOrder;

            var users = await _userService.GetUsersQueried(searchTerm, sortOrder, pageIndex, PageSize);

            return View("~/Views/Users/Index.cshtml", users);
        }
        [AllowAnonymous] //To be removed if the flow is finalized
        public IActionResult AddUser()
        {
            return View("~/Views/Users/AddUser.cshtml");
        }
        //[AllowAnonymous] //To be removed if the flow is finalized
        /* public IActionResult EditUser()
         {
             return View("~/Views/Users/EditUser.cshtml");
         }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return View(model);
            }

            try
            {
                await _userService.AddUserFromAdmin(model);
                TempData["SuccessMessage"] = "User added successfully!";
                return RedirectToAction("AddUser");
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("~/Views/Users/AddUser.cshtml");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return View("~/Views/Users/AddUser.cshtml");
            }
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await _userService.DeleteUser(id);

                if (!deleted)
                {
                    TempData["ErrorMessage"] = "User could not be deleted.";
                }
                else
                {
                    TempData["SuccessMessage"] = "User deleted successfully.";
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = $"An error occurred while deleting the user: {e.Message}";
            }

            return RedirectToAction("Index");

        }

        [HttpGet]
        [Route("User/EditUser/{id}")]
        public async Task<IActionResult> EditUser(int id)
        {
            try
            {
                // Add logging to check the ID being received
                Console.WriteLine($"Attempting to edit user with ID: {id}");

                var user = await _userService.GetUserById(id);

                // Add logging to check if user was found
                if (user == null)
                {
                    Console.WriteLine($"User with ID {id} not found in the database.");
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("Index");
                }

                Console.WriteLine($"User found: ID={user.Id}, Email={user.Email}, Username={user.UserName}");

                var viewModel = new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    IsEmailVerified = user.IsEmailVerified,
                    IsUpdate = true // Mark as update to disable validation
                };

                return View("~/Views/Users/EditUser.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user {id}: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "An error occurred while loading the user.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, UserViewModel model)
        {
            // Ensure the ID is populated
            model.Id = id;

            // If updating and no password is provided, remove validation errors for password fields
            if (model.IsUpdate && string.IsNullOrEmpty(model.Password))
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return View("~/Views/Users/EditUser.cshtml", model);
            }

            try
            {
                await _userService.UpdateUser(model);
                ViewData["SaveSuccess"] = true;
                return View("~/Views/Users/EditUser.cshtml", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("~/Views/Users/EditUser.cshtml", model);
            }
        }

        [HttpGet]
        [Route("User/CheckUsernameAvailability")]
        public async Task<IActionResult> CheckUsernameAvailability(string username, int? currentUserId)
        {
            if (string.IsNullOrEmpty(username))
            {
                return Json(new { isAvailable = false, message = "Username is required" });
            }

            try
            {
                // Skip checking against the current user when editing
                bool isAvailable = await _userService.IsUsernameAvailable(username, currentUserId);

                return Json(new
                {
                    isAvailable = isAvailable,
                    message = isAvailable ? "" : $"A user with this username already exists!"
                });
            }
            catch (Exception ex)
            {
                return Json(new { isAvailable = false, message = "Error checking username" });
            }
        }

    }
}
