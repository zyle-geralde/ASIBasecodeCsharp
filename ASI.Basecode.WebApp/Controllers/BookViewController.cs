using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASI.Basecode.WebApp.Controllers
{
    //This controller will handle actions that return views
    public class BookViewController : Controller
    {
        [HttpGet]
        [Route("Book/AddBook")]
        [AllowAnonymous]// Bypass authorization. No need to Log In 
        public IActionResult AddBook()
        {
            // This will look for a view at /Views/Books/AddBook.cshtml
            return View("~/Views/Books/AddBook.cshtml");
        }
    }
}
