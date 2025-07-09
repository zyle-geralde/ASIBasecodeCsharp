using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.AccessControl;
using ASI.Basecode.WebApp.Payload.BooksPayload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers.BookControllerFolder
{
    //This controller will handle actions that return views
    public class BookViewController : Controller
    {

        private readonly IBookService _bookService;
        private readonly IReviewService _reviewService;
        private readonly IBookGenreService _bookGenreService;
        private readonly IAccessControlInterface _accessControlInterface;
        private readonly IAuthorRepository _authorRepository;
        private readonly IAuthorService _authorService;
        private readonly ILanguageService _languageService;
        //private readonly IBookRepository _bookRepository;
        public BookViewController(IBookService bookService, IReviewService reviewService, IBookGenreService bookGenreService, IAccessControlInterface accessControlInterface,IAuthorRepository authorRepository, IAuthorService authorService, ILanguageService languageService)
        {
            _bookService = bookService;
            _reviewService = reviewService;
            _bookGenreService = bookGenreService;
            _accessControlInterface = accessControlInterface;
            _authorRepository = authorRepository;
            _authorService = authorService;
            _languageService = languageService;
        }




        [HttpGet]
        [Route("Book/AddBook")]
        [Authorize]// Bypass authorization. No need to Log In 
        public async Task<IActionResult> AddBook()
        {

            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");
            // This will look for a view at /Views/Books/AddBook.cshtml
            return View("~/Views/Books/AddBook.cshtml");
        }

        [HttpGet]
        [Route("Book/GetGenre")]
        [Authorize]
        public async Task<IActionResult> GetAllGenres()
        {

            try
            {
                List<string> all_genres = await _bookService.GetAllGenres();

                return Ok(new { Message = all_genres });
            }
            catch (Exception ex) 
            {
                return StatusCode(500, $"Failed to get all Genres: {ex.Message}");
            }
        }


        [HttpGet]
        [Route("Book/GetLanguage")]
        [Authorize]
        public async Task<IActionResult> GetAllLanguage()
        {

            try
            {
                List<string> all_language = await _bookService.GetAllLanguage();

                return Ok(new { Message = all_language });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to get all Genres: {ex.Message}");
            }
        }


        [HttpGet]
        [Route("Book/ListBook")]
        [Authorize]
        public async Task<IActionResult> ListBook(
            string? searchTerm,
            string? author,
            int? rating,
            DateTime? publishedFrom,
            DateTime? publishedTo,
            string[]? genreFilter,
            string? languageFilter,
            string? sortOrder,
            bool sortDescending = true,
            bool? isFeatured = null,

            int? page = 1
            )
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");
            const int PageSize = 10;
            int pageIndex = page.GetValueOrDefault(1);
            string authorId = await _authorRepository.GetAuthorByName(author != null ? author : "");
            string authorIdFromSearch = await _authorRepository.GetAuthorByName(searchTerm != null ? searchTerm : "");


            var queryParams = new BookQueryParams
            {
                SearchTerm = searchTerm,
                SearchAuhtor = !string.IsNullOrEmpty(authorIdFromSearch) ? authorIdFromSearch : "",
                Author = !string.IsNullOrEmpty(authorId)?authorId:"",
                Rating = rating,
                PublishedFrom = publishedFrom,
                PublishedTo = publishedTo,
                GenreNames = genreFilter?.ToList(),
                Language = languageFilter,
                SortDescending = sortDescending,
                PageIndex = page.GetValueOrDefault(1),
                IsFeatured = isFeatured,
                SortOrder = sortOrder ?? "updateddate",
                PageSize = PageSize,


            };


            ViewData["CurrentSearch"] = searchTerm;
            ViewData["CurrentAuthor"] =author;
            ViewData["CurrentRating"] = rating;
            ViewData["CurrentFromDate"] = publishedFrom?.ToString("yyyy-MM-dd");
            ViewData["CurrentToDate"] = publishedTo?.ToString("yyyy-MM-dd");
            ViewData["CurrentGenres"] = genreFilter ?? Array.Empty<string>();
            ViewData["CurrentSort"] = queryParams.SortOrder;
            ViewData["CurrentSortDescending"] = queryParams.SortDescending;
            ViewData["CurrentIsFeatured"] = isFeatured;
            ViewData["CurrentLanguage"] = languageFilter;


            var allGenres = await _bookGenreService.GetAllGenreList();
            var allLanguages = await _languageService.GetAllLanguage();
            ViewData["AllGenres"] = allGenres;
            ViewData["AllLanguages"] = allLanguages;


            //var books = await _bookService.GetBooks(
            //    queryParams );
            PaginatedList<BookViewModel> books = await _bookService.GetBooks(queryParams);
            //List<Book> books = await _bookService.GetAllBooks();
            return View("~/Views/Books/ListBook.cshtml", books);
        }

        [HttpGet]
        [Route("Book/SearchResults")]
        [Authorize]
        public async Task<IActionResult> SearchResults(
        string? searchTerm,
        string? author,
        int? rating,
        DateTime? publishedFrom,
        DateTime? publishedTo,
        string[]? genreFilter,
        string? languageFilter,
        string? sortOrder,
        bool sortDescending = false,
        bool? isFeatured = null,
        int? page = 1,
        string? category = null
        )
        {
                const int PageSize = 18;
                int pageIndex = page.GetValueOrDefault(1);

                var queryParams = new BookQueryParams
                {
                    SearchAuhtor = searchTerm ?? "",
                    SearchTerm = searchTerm,
                    Author = author,
                    Rating = rating,
                    PublishedFrom = publishedFrom,
                    PublishedTo = publishedTo,
                    GenreNames = genreFilter?.ToList(),
                    Language = languageFilter,
                    IsFeatured = isFeatured,
                    SortDescending = sortDescending,
                    PageIndex = page.GetValueOrDefault(1),
                    SortOrder = sortOrder ?? "title",
                    PageSize = PageSize,


                };


                ViewData["CurrentSearch"] = searchTerm;
                ViewData["CurrentAuthor"] = author;
                ViewData["CurrentRating"] = rating;
                ViewData["CurrentFromDate"] = publishedFrom?.ToString("yyyy-MM-dd");
                ViewData["CurrentToDate"] = publishedTo?.ToString("yyyy-MM-dd");
                ViewData["CurrentGenres"] = genreFilter ?? Array.Empty<string>();
                ViewData["CurrentIsFeatured"] = isFeatured;
                ViewData["CurrentSort"] = queryParams.SortOrder;
                ViewData["CurrentSortDescending"] = queryParams.SortDescending;
                ViewData["CurrentLanguage"] = languageFilter;

            var allGenres = await _bookGenreService.GetAllGenreList();
                ViewData["AllGenres"] = allGenres;
            var allLanguages = await _languageService.GetAllLanguage();
            ViewData["AllLanguages"] = allLanguages;
            if (!string.IsNullOrEmpty(category))
            {
                ViewData["CategoryTitle"] = category;
            }
            else if (sortOrder?.Equals("AverageRating", StringComparison.OrdinalIgnoreCase) == true)
            {
                ViewData["CategoryTitle"] = "Top Rated";
            }
            else if (sortOrder?.Equals("UploadDate", StringComparison.OrdinalIgnoreCase) == true)
            {
                ViewData["CategoryTitle"] = "Newly Added";
            }
            else
            {
                ViewData["CategoryTitle"] = "Search Results";
            }



            PaginatedList<BookViewModel> books = await _bookService.GetBooks(queryParams);
            return View("~/Views/Books/BookSearchResults.cshtml", books);
            }


        [HttpGet]
        [Route("Book/EditBook/{bookId}")]
        [Authorize]
        public async Task<IActionResult> EditBook(string bookId)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            BookViewModel book = await _bookService.GetBookById(bookId);
           return View("~/Views/Books/EditBook.cshtml",book);
        }

        [HttpGet]
        [Route("Book/BookDetails/{bookId}", Name="BookDetails")]
        [Authorize]
        public async Task<IActionResult> GetBook(string bookId)
        {


            BookViewModel book = await _bookService.GetBookById(bookId);
            if(book == null)
            {
                return NotFound();
            }

            var reviews= await _reviewService.GetReviewsByBookId(bookId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool hasReviewed = reviews.Any(r => r.UserId == userId);
            Author authorName = await _authorRepository.GetAuthorById(book != null ? book.Author : "");
            AuthorViewModel authorObject = await _authorService.GetAuthorById(book != null ? book.Author : "");
            var bookDetails = new BookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Subtitle = book.Subtitle,
                UploadDate = book.UploadDate,
                UpdatedDate = book.UpdatedDate,
                PublicationDate = book.PublicationDate,
                Publisher = book.Publisher,
                PublicationLocation = book.PublicationLocation,
                Description = book.Description,
                NumberOfPages = book.NumberOfPages,
                Language = book.Language,
                CoverImageUrl = book.CoverImageUrl,
                GenreList = book.GenreList,
                BookFileUrl = book.BookFileUrl,
                SeriesName = book.SeriesName,
                SeriesOrder = book.SeriesOrder,
                SeriesDescription = book.SeriesDescription,
                AverageRating = book.AverageRating,
                CreatedBy = book.CreatedBy,
                UpdatedBy = book.UpdatedBy,
                Author = authorName != null ? authorName.AuthorName : "",
                Likes = book.Likes,
                ISBN10 = book.ISBN10,
                ISBN13 = book.ISBN13,
                Edition = book.Edition,
                IsFeatured = book.IsFeatured,
                HasReviewed = hasReviewed,
                Reviews = reviews
                            .Select(r => new ReviewViewModel
                            {
                                ReviewId = r.ReviewId,
                                UserId = r.UserId,
                                Rating = r.Rating,
                                Comment = r.Comment,
                                UploadDate = r.UploadDate
                            })
                            .ToList()
            };

            ViewBag.AuthorDetails = authorObject;


            return View("~/Views/Books/BookDetails.cshtml", bookDetails);
        }

        [HttpPost]
        [Route("Book/AddBook")]
        [Authorize]
        public async Task<IActionResult> AddBook(BookViewModel book)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                try
                {
                    await _bookService.AddBook(book);

                    //TempData["message"] = "Book added successfully!";
                    //return RedirectToAction("ListBook"); 
                    return Ok(new { Message = "Book Edited successfully!" });
                }
                catch (Exception ex)
                {
                    //ModelState.AddModelError("", "An unexpected error occurred: " + ex.Message);
                    return BadRequest(new { ex.Message });
                }
            }

            //return View("~/Views/Books/AddBook.cshtml", book);
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { errors = errors, Message = "Validation failed." });
        }

        [HttpPost]
        [Route("Book/EditBook")]
        [Authorize]
        public async Task<IActionResult> EditBook(BookViewModel book)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                book.UpdatedDate = DateTime.UtcNow;
                book.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                try
                {
                    await _bookService.EditBook(book);
                    return Ok(new { Message = "Book Edited successfully!" });
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error updating book: {ex.Message}");
                    return StatusCode(500, new {Message = ex.Message });
                }
            }


            //return bad request if ModelState is not valid
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { errors = errors, Message = "Validation failed." });
        }

        [HttpPost]
        [Route("Book/Delete")]
        [Authorize]

        public async Task<IActionResult> DeleteBook([FromBody] DeleteBookPayload book)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");


            if (book == null)
            {
                return BadRequest(new { Message = "No data has been passed" });
            }

            try
            {
                await _bookService.DeletBook(book.BookId);
                return Ok(new { Message = "Book Deleted successfully!" });
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
        [Route("Book/GetAuthor")]
        [Authorize]
        public async Task<IActionResult> GetAuthor()
        {

            try
            {
                List<string> all_author = await _bookService.GetAllAuthor();

                return Ok(new { Message = all_author });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to get all Authors: {ex.Message}");
            }
        }
    }
}
