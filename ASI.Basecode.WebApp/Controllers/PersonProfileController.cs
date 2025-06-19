using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class PersonProfileController : Controller
    {
        private readonly IPersonProfileService _personProfileService;
        private readonly IUserService _userService;

        public PersonProfileController(IPersonProfileService profileService, IUserService userService)
        {
            _personProfileService = profileService;
            _userService = userService;
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

            var vm = new PersonProfileViewModel
            {
                UserId = profile.ProfileID,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                AboutMe = profile.AboutMe
            };

            return View(vm);
        }
    }
}
