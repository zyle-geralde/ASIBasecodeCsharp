using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.WebApp.Controllers
{
    public class AuthorController : Controller
    {

        [HttpGet]
        [Route("Author/Index")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [Route("Author/AddAuthor")] 
        [AllowAnonymous]
        public IActionResult AddAuthor()
        {
            return View("~/Views/Author/AddAuthor.cshtml");
        }
    }
}
