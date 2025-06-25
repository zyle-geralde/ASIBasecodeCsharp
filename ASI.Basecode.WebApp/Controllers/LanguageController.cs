using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using ASI.Basecode.Services.ServiceModels;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class LanguageController : Controller
    {

        private readonly ILanguageService _languageService;

        public LanguageController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpGet]
        [Route("Language/Index")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View("~/Views/Language/Index.cshtml", new LanguageViewModel());
        }

        [HttpPost]
        [Route("Language/AddLanguage")]
        [AllowAnonymous]
        public async Task<IActionResult> AddLanguage(LanguageViewModel language)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _languageService.AddLanguage(language);
                    TempData["SuccessMessage"] = "Langugae added successfully!";
                    return RedirectToAction("Index");
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("",   ex.Message);
                }
                catch (ApplicationException ex)
                {
                     ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                     ModelState.AddModelError("", ex.Message);
                }

            }

            return View("~/Views/Language/Index.cshtml",language);
        }
    }
}
