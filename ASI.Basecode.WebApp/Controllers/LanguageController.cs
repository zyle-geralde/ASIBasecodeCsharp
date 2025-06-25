using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASI.Basecode.WebApp.Controllers
{
    public class LanguageController : Controller
    {

        [HttpGet]
        [Route("Language/Index")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
    }
}
