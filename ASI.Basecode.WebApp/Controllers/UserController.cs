using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.AccessControl;
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
        private readonly IAccessControlInterface _accessControlInterface;
        public UserController(IUserService userService,IAccessControlInterface accessControlInterface)
        {
            _userService = userService;
            _accessControlInterface = accessControlInterface;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index(string searchTerm, string sortOrder, int? page)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            const int PageSize = 10;

            int pageIndex = page.GetValueOrDefault(1);
            ViewData["CurrentSearch"] = searchTerm;
            ViewData["CurrentSort"] = sortOrder;

            var users = await _userService.GetUsersQueried(searchTerm, sortOrder, pageIndex, PageSize);

            return View("~/Views/Users/Index.cshtml", users);
        }
        [Authorize] //To be removed if the flow is finalized
        public async Task<IActionResult> AddUser()
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            return View("~/Views/Users/AddUser.cshtml");
        }
        [Authorize] //To be removed if the flow is finalized
        public async Task<IActionResult> EditUser()
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");
            return View("~/Views/Users/EditUser.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddUser(UserViewModel model)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

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
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return View(model);
            }
        }


        [HttpPost]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

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

    }
}
