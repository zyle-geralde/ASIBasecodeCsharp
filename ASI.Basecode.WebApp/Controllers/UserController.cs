using ASI.Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    }
}
