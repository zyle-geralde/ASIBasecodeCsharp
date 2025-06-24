using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Payload.BookGenrePayload;
using ASI.Basecode.WebApp.Payload.BooksPayload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class BookGenreController : Controller
    {
        private readonly IBookGenreService BookGenreService;
        private readonly IBookService BookService;

        public BookGenreController(IBookGenreService book_genre_service,IBookService book_service)
        {
            BookGenreService = book_genre_service;
            BookService = book_service;
        }

        [HttpGet]
        [Route("BookGenre/AddGenre")]
        [AllowAnonymous]

        public IActionResult AddGenre()
        {
            return View("~/Views/BookGenres/AddGenre.cshtml");
        }

        [HttpPost]
        [Route("BookGenre/AddGenre")]
        [AllowAnonymous]

        public async Task<IActionResult> AddGenre(BookGenreViewModel book_genre)
        {
            if(ModelState.IsValid){
                try
                {
                    await BookGenreService.AddGenre(book_genre);
                    return Ok(new { Message = "Genre Successfully added" });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { Message = ex.Message });
                }
                catch (ApplicationException ex)
                {
                    return BadRequest(new { Message = ex.Message });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }

            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { errors = errors, Message = "Validation failed." });


        }
        [HttpGet]
        [Route("BookGenre/ListGenre")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBookGenres()
        {
            try
            {
                List<BookGenreViewModel> book_genre_list = await BookGenreService.GetAllGenreList();

                return View("~/Views/BookGenres/@{\r\n    Layout = \"_AdminSidebarLayout\";\r\n\r\n    // Example data for demonstration\r\n    var books = Enumerable.Range(1, 10).Select(i => new\r\n    {\r\n        Title = \"The Lost Expedition\",\r\n        Author = \"Tom Hardy, 2020\",\r\n        Image = Url.Content(\"~/img/bookcover_container.png\"),\r\n        Rating = 5\r\n    }).ToList();\r\n\r\n    int cardsPerSlide = 5;\r\n    int totalSlides = (int)Math.Ceiling(books.Count / (double)cardsPerSlide);\r\n}\r\n\r\n@section styles {\r\n    <link href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css\" rel=\"stylesheet\" />\r\n    <link rel=\"stylesheet\" href=\"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css\">\r\n    <link rel=\"stylesheet\" href=\"~/css/admin/common.css\" asp-append-version=\"true\" />\r\n    <link rel=\"stylesheet\" href=\"~/css/admin/dashboard.css\" asp-append-version=\"true\" />\r\n    <style>\r\n        .stats-card {\r\n            cursor: pointer;\r\n        }\r\n    </style>\r\n}\r\n\r\n<div class=\"admin-common-container\">\r\n    <div class=\"admin-common-content-wrapper\">\r\n        <div class=\"header-section\">\r\n            <h1>Dashboard</h1>\r\n            <p>Overview of your book review platform</p>\r\n        </div>\r\n\r\n        <!-- Stats Cards -->\r\n        <div class=\"row g-4 mb-4\">\r\n            <div class=\"col-md-6 col-lg-3\">\r\n                <div class=\"stats-card a\" onclick=\"window.location.href='@Url.Action(\"Index\", \"Books\")'\">\r\n                    <div class=\"d-flex justify-content-between align-items-center\">\r\n                        <div>\r\n                            <h3 class=\"mb-0\">367K</h3>\r\n                            <p class=\"text-muted mb-0\">Total Reviews</p>\r\n                        </div>\r\n                        <i class=\"fas fa-star stats-icon\"></i>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n            <div class=\"col-md-6 col-lg-3\">\r\n                <div class=\"stats-card b\" onclick=\"window.location.href='@Url.Action(\"Index\", \"Books\")'\">\r\n                    <div class=\"d-flex justify-content-between align-items-center\">\r\n                        <div>\r\n                            <h3 class=\"mb-0\">12K</h3>\r\n                            <p class=\"text-muted mb-0\">Total Books</p>\r\n                        </div>\r\n                        <i class=\"fas fa-book stats-icon\"></i>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n            <div class=\"col-md-6 col-lg-3\">\r\n                <div class=\"stats-card a\" onclick=\"window.location.href='@Url.Action(\"Index\", \"Users\")'\">\r\n                    <div class=\"d-flex justify-content-between align-items-center\">\r\n                        <div>\r\n                            <h3 class=\"mb-0\">123K</h3>\r\n                            <p class=\"text-muted mb-0\">Total Users</p>\r\n                        </div>\r\n                        <i class=\"fas fa-users stats-icon\"></i>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n            <div class=\"col-md-6 col-lg-3\">\r\n                <div class=\"stats-card b\" onclick=\"window.location.href='@Url.Action(\"Index\", \"BookGenre\")'\">\r\n                    <div class=\"d-flex justify-content-between align-items-center\">\r\n                        <div>\r\n                            <h3 class=\"mb-0\">25</h3>\r\n                            <p class=\"text-muted mb-0\">Total Genres</p>\r\n                        </div>\r\n                        <i class=\"fas fa-tags stats-icon\"></i>\r\n                    </div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n\r\n        <!-- Newly Added Books Section -->\r\n        <div class=\"section mb-4\">\r\n            <div class=\"d-flex justify-content-between align-items-center mb-3\">\r\n                <h2 class=\"section-title\">Newly Added Books</h2>\r\n                <a href=\"#\" class=\"view-more\">View more <i class=\"fas fa-arrow-right\"></i></a>\r\n            </div>\r\n            <div class=\"carousel-container\">\r\n                <div id=\"newBooksCarousel\" class=\"carousel slide\" data-bs-ride=\"carousel\" data-bs-interval=\"3000\">\r\n                    <div class=\"carousel-inner\">\r\n                        @for (int slide = 0; slide < totalSlides; slide++)\r\n                        {\r\n                            <div class=\"carousel-item @(slide == 0 ? \"active\" : \"\")\">\r\n                                <div class=\"row carousel-row justify-content-center\">\r\n                                    @for (int card = 0; card < cardsPerSlide; card++)\r\n                                    {\r\n                                        int bookIndex = slide * cardsPerSlide + card;\r\n                                        if (bookIndex >= books.Count) { break; }\r\n                                        var book = books[bookIndex];\r\n                                        <div class=\"book-card-wrapper\">\r\n                                            <div class=\"book-card\">\r\n                                                <img src=\"@book.Image\" class=\"book-cover\" alt=\"Book cover\">\r\n                                                <div class=\"book-info\">\r\n                                                    <h5>@book.Title</h5>\r\n                                                    <p class=\"author\">@book.Author</p>\r\n                                                    <div class=\"rating\">\r\n                                                        @for (int star = 0; star < book.Rating; star++)\r\n                                                        {\r\n                                                            <i class=\"fas fa-star\"></i>\r\n                                                        }\r\n                                                    </div>\r\n                                                </div>\r\n                                            </div>\r\n                                        </div>\r\n                                    }\r\n                                </div>\r\n                            </div>\r\n                        }\r\n                    </div>\r\n                    <button class=\"carousel-control-prev\" type=\"button\" data-bs-target=\"#newBooksCarousel\" data-bs-slide=\"prev\">\r\n                        <span class=\"carousel-control-prev-icon\" aria-hidden=\"true\"></span>\r\n                        <span class=\"visually-hidden\">Previous</span>\r\n                    </button>\r\n                    <button class=\"carousel-control-next\" type=\"button\" data-bs-target=\"#newBooksCarousel\" data-bs-slide=\"next\">\r\n                        <span class=\"carousel-control-next-icon\" aria-hidden=\"true\"></span>\r\n                        <span class=\"visually-hidden\">Next</span>\r\n                    </button>\r\n                </div>\r\n            </div>\r\n        </div>\r\n\r\n        <!-- Top 10 Rated Books Section -->\r\n        <div class=\"section\">\r\n            <div class=\"d-flex justify-content-between align-items-center mb-3\">\r\n                <h2 class=\"section-title\">Top 10 Rated Books</h2>\r\n                <a href=\"#\" class=\"view-more\">View more <i class=\"fas fa-arrow-right\"></i></a>\r\n            </div>\r\n            <div class=\"carousel-container\">\r\n                <div id=\"topBooksCarousel\" class=\"carousel slide\" data-bs-ride=\"carousel\" data-bs-interval=\"3000\">\r\n                    <div class=\"carousel-inner\">\r\n                        @for (int slide = 0; slide < totalSlides; slide++)\r\n                        {\r\n                            <div class=\"carousel-item @(slide == 0 ? \"active\" : \"\")\">\r\n                                <div class=\"row carousel-row justify-content-center\">\r\n                                    @for (int card = 0; card < cardsPerSlide; card++)\r\n                                    {\r\n                                        int bookIndex = slide * cardsPerSlide + card;\r\n                                        if (bookIndex >= books.Count) { break; }\r\n                                        var book = books[bookIndex];\r\n                                        <div class=\"book-card-wrapper\">\r\n                                            <div class=\"book-card\">\r\n                                                <img src=\"@book.Image\" class=\"book-cover\" alt=\"Book cover\">\r\n                                                <div class=\"book-info\">\r\n                                                    <h5>@book.Title</h5>\r\n                                                    <p class=\"author\">@book.Author</p>\r\n                                                    <div class=\"rating\">\r\n                                                        @for (int star = 0; star < book.Rating; star++)\r\n                                                        {\r\n                                                            <i class=\"fas fa-star\"></i>\r\n                                                        }\r\n                                                    </div>\r\n                                                </div>\r\n                                            </div>\r\n                                        </div>\r\n                                    }\r\n                                </div>\r\n                            </div>\r\n                        }\r\n                    </div>\r\n                    <button class=\"carousel-control-prev\" type=\"button\" data-bs-target=\"#topBooksCarousel\" data-bs-slide=\"prev\">\r\n                        <span class=\"carousel-control-prev-icon\" aria-hidden=\"true\"></span>\r\n                        <span class=\"visually-hidden\">Previous</span>\r\n                    </button>\r\n                    <button class=\"carousel-control-next\" type=\"button\" data-bs-target=\"#topBooksCarousel\" data-bs-slide=\"next\">\r\n                        <span class=\"carousel-control-next-icon\" aria-hidden=\"true\"></span>\r\n                        <span class=\"visually-hidden\">Next</span>\r\n                    </button>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n\r\n@section Scripts {\r\n    <script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js\"></script>\r\n}.cshtml", book_genre_list);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet]
        [Route("BookGenre/EditGenre/{genre_id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBookGenreById(string genre_id)
        {
            try
            {
                BookGenreViewModel retreived_genre = await BookGenreService.GetBookGenreById(genre_id);

                return View("~/Views/BookGenres/EditGenre.cshtml", retreived_genre);
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("BookGenre/EditGenre")]
        [AllowAnonymous]
        public async Task<IActionResult> EditGenre(BookGenreViewModel book_genre)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    await BookGenreService.EditGenre(book_genre);
                    return Ok(new { Message = "Genre Successfully edited" });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { Message = ex.Message });
                }
                catch (ApplicationException ex)
                {
                    return BadRequest(new { Message = ex.Message });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }

            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { errors = errors, Message = "Validation failed." });
        }


        [HttpPost]
        [Route("BookGenre/DeleteGenre")]
        [AllowAnonymous]

        public async Task<IActionResult> DeleteBook([FromBody] DeleteGenrePayload genre)
        {
            if (genre == null)
            {
                return BadRequest(new { Message = "No data has been passed" });
            }

            try
            {
                await BookGenreService.DeleteGenre(genre.BookGenreId);
                return Ok(new { Message = "Genre Deleted successfully!" });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet]
        [Route("BookGenre/GenreView/{genre_name}")]
        [AllowAnonymous]

        public async Task<IActionResult> GetBooksByGenre(string genre_name)
        {
            try
            {
                //Change this during code cleaning
                List<BookViewModel> retreived_books_by_genre = await BookGenreService.GetBooksByGenre(genre_name);
                BookGenreViewModel retreived_genre_by_genre_name = await BookGenreService.GetBookGenreByName(genre_name);

                ViewBag.CurrentGenreDetails = retreived_genre_by_genre_name;

                return View("~/Views/Books/ListBook.cshtml", retreived_books_by_genre);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

    }
}
