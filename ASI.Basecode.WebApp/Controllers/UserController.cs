using ASI.Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public IActionResult Index()
        {
            var model = _userService.GetAllUsers();
            return View("~/Views/Users/Index.cshtml", model);
        }
        [AllowAnonymous] //To be removed if the flow is finalized
        public IActionResult AddUser()
        {
            return View();
        }
        [AllowAnonymous] //To be removed if the flow is finalized
        public IActionResult EditUser()
        {
            return View();
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
                    TempData["ErrorMessage"] = "Usercould not be deleted.";
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
