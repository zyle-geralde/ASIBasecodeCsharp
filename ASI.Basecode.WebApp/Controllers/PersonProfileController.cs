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

        public PersonProfileController(IPersonProfileService profileService)
        {
            _personProfileService = profileService;
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
                MiddleName = profile.MiddleName,
                LastName = profile.LastName,
                AboutMe = profile.AboutMe,
                Birthdate = profile.BirthDate,
                Gender = profile.Gender,
                Location = profile.Location,
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
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PersonProfileViewModel model)
        {
            if(!ModelState.IsValid)
                return View("~/Views/PersonProfile/Edit.cshtml", model);

            var updated = await _personProfileService.EditPersonProfile(model);
            if (!updated)
                return NotFound();

            return RedirectToAction("Index");

        }

    }
}
