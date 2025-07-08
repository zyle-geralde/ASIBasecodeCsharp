using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.AccessControl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAccessControlInterface _accessControlInterface;
        public UserController(IUserService userService,IAccessControlInterface accessControlInterface)
        {
            _userService = userService;
            _accessControlInterface = accessControlInterface;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index(string? searchTerm, string? role, string? sortOrder, bool sortDescending = true, int? page = 1)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");
            if (TempData["SuccessMessage"] != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
            }
            const int PageSize = 10;
            int pageIndex = page.GetValueOrDefault(1);

            var queryParams = new UserQueryParams
            {
                SearchTerm = searchTerm,
                Role = role,
                SortOrder = sortOrder ?? "createdtime",
                SortDescending = sortDescending,
                PageIndex = pageIndex,
                PageSize = PageSize
            };
            ViewData["CurrentSearch"] = searchTerm;
            ViewData["CurrentSort"] = queryParams.SortOrder;
            ViewData["CurrentSortDescending"] = queryParams.SortDescending;
            ViewData["CurrentRoleFilter"] = queryParams.Role;
            PaginatedList<User> users = await _userService.GetUsersQueried(queryParams);

            return View("~/Views/Users/Index.cshtml", users);
        }
        [HttpGet]
        [Authorize] 
        public async Task<IActionResult> AddUser(string role="User")
        {


            if (!IsPostBack())
            {
                TempData.Remove("SuccessMessage");
                ViewData["ShowSuccessModal"] = null;
                ViewData["SaveSuccess"] = null;
            }
            var vm = new UserViewModel
            {
                Role = role
            };
            return View("~/Views/Users/AddUser.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddUser(UserViewModel model)
        {


            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return View("~/Views/Users/AddUser.cshtml", model);
            }

            try
            {
                await _userService.AddUserFromAdmin(model);

                // Return directly to the view with success flags instead of redirecting
                ModelState.Clear(); // Clear the form

                // Use ViewData instead of TempData for immediate display
                TempData["ShowSuccessModal"] = true;
                TempData["SuccessMessage"] = "User added successfully!";
                return RedirectToAction("Index");
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("~/Views/Users/AddUser.cshtml", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return View("~/Views/Users/AddUser.cshtml", model);
            }
        }

        [HttpPost]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> EditUser(int id)
        {

            try
            {
                if (!IsPostBack())
                {
                    TempData.Remove("SuccessMessage");
                    ViewData["SaveSuccess"] = null;
                }
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
        [Authorize]
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


        private bool IsPostBack()
        {
            return Request.Method == "POST" ||
                   (Request.Headers["Referer"].ToString()?.Contains(Request.Path.Value) == true);
        }

    }
}
