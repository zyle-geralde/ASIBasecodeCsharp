using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.QueryParams;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.AccessControl;
using ASI.Basecode.WebApp.Payload.AuthorPayload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IAuthorService _authorService;
        private readonly IBookService _bookService;
        private readonly IAccessControlInterface _accessControlInterface;

        public AuthorController(IAuthorService authorService, IBookService book_service, IAccessControlInterface accessControlInterface)
        {
            _authorService = authorService;
            _bookService = book_service;
            _accessControlInterface = accessControlInterface;
        }

        [HttpGet]
        [Route("Author/Index")]
        [Authorize]
        public async Task<IActionResult> Index(string? searchTerm, string? sortOrder, bool sortDescending = false, int? page = 1)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            try
            {
                const int pageSize = 10;
                int pageIndex = page.GetValueOrDefault(1);

                var queryParams = new AuthorQueryParams
                {
                    SearchTerm = searchTerm,
                    SortOrder = sortOrder ?? "name",
                    SortDescending = sortDescending,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                ViewData["CurrentSearch"] = searchTerm;
                ViewData["CurrentSort"] = queryParams.SortOrder;
                ViewData["CurrentSortDescending"] = queryParams.SortDescending;
                PaginatedList<Author> authorList = await _authorService.GetAuthorQueried(queryParams);


                return View("~/Views/Author/Index.cshtml", authorList);
            }
            catch (ApplicationException)
            {

                return View("~/Views/Author/Index.cshtml", new List<LanguageViewModel>());
            }
            catch (Exception)
            {
                return View("~/Views/Author/Index.cshtml", new List<LanguageViewModel>());

            }
        }


        [HttpGet]
        [Route("Author/AddAuthor")]
        [Authorize]
        public async Task<IActionResult> AddAuthor()
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            return View("~/Views/Author/AddAuthor.cshtml");
        }

        [HttpGet]
        [Route("Author/EditAuthor/{authorId}")]
        [Authorize]
        public async Task<IActionResult> EditAuthor(string authorId)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            try
            {
                AuthorViewModel retreivedAuthor = await _authorService.GetAuthorById(authorId);

                return View("~/Views/Author/EditAuthor.cshtml", retreivedAuthor);
            }
            catch (ArgumentNullException ex)
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
        [Route("Author/AddAuthor")]
        [Authorize]
        public async Task<IActionResult> AddAuthor(AuthorViewModel author)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                try
                {
                    await _authorService.AddAuthor(author);
                    return Ok(new { Message = "Author Successfully added" });
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
        [Route("Author/EditAuthor")]
        [Authorize]
        public async Task<IActionResult> EditAuthor(AuthorViewModel author)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                try
                {
                    await _authorService.EditAuthor(author);
                    return Ok(new { Message = "Author Successfully edited" });
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
        [Route("Author/DeleteAuthor")]
        [Authorize]

        public async Task<IActionResult> DeleteAuthor([FromBody] DeleteAuthorPayload author)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            if (author == null)
            {
                return BadRequest(new { Message = "Payload is null or empty" });
            }

            try
            {
                await _authorService.DeleteAuthor(author.AuthorId);
                return Ok(new { Message = "Author Deleted successfully!" });
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
        [Route("Author/AuthorView/{authorId}")]
        [Authorize]

        public async Task<IActionResult> GetBooksByAuthor(string authorId)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            try
            {
                List<BookViewModel> retreivedBooksByAuthor = await _authorService.GetBooksByAuthor(authorId);
                AuthorViewModel retreivedAuthorById = await _authorService.GetAuthorById(authorId);

                ViewBag.CurrentAuthorDetails = retreivedAuthorById;

                return View("~/Views/Books/ListBook.cshtml", retreivedBooksByAuthor);
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
