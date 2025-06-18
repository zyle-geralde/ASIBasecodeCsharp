using ASI.Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASI.Basecode.WebApp.Controllers
{
    public class BookGenreController : Controller
    {
        private readonly IBookGenreService BookGenreService;

        public BookGenreController(IBookGenreService book_genre_service)
        {
            BookGenreService = book_genre_service;
        }

        [HttpGet]
        [Route("BookGenre/AddGenre")]
        [AllowAnonymous]

        public IActionResult AddGenre()
        {
            return View("~/Views/BookGenres/AddGenre.cshtml");
        }
    }
}
