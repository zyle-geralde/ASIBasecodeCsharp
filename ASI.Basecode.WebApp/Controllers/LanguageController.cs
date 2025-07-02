using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.AccessControl;
using ASI.Basecode.WebApp.Payload.BooksPayload;
using ASI.Basecode.WebApp.Payload.LanguagePayload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers
{
    public class LanguageController : Controller
    {

        private readonly ILanguageService _languageService;
        private readonly IAccessControlInterface _accessControlInterface;
        private readonly IBookGenreService _bookGenreService;

        public LanguageController(ILanguageService languageService,IAccessControlInterface accessControlInterface, IBookGenreService bookGenreService)
        {
            _languageService = languageService;
            _accessControlInterface = accessControlInterface;
            _bookGenreService = bookGenreService;
        }


        [HttpGet]
        [Route("Language/Index")]
        [Authorize]
        public async Task<IActionResult> LanguageList()
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            try
            {
                List<LanguageViewModel> language_list = await _languageService.GetAllLanguage();

                return View("~/Views/Language/Index.cshtml", language_list);
            }
            catch (ApplicationException ex)
            {

                return View("~/Views/Language/Index.cshtml", new List<LanguageViewModel>());
            }
            catch (Exception ex)
            {
                return View("~/Views/Language/Index.cshtml", new List<LanguageViewModel>());
                
            }
        }

        [HttpPost]
        [Route("Language/AddLanguage")]
        [Authorize]
        public async Task<IActionResult> AddLanguage([FromBody]LanguageViewModel language)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                try
                {
                    await _languageService.AddLanguage(language);
                    return Ok(new { Message = "Language Added successfully!" });
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(new { ex.Message });
                }
                catch (ApplicationException ex)
                {
                    return BadRequest(new { ex.Message });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { ex.Message });
                }

            }


            return BadRequest(new { Message = "Title should not be null or exceed 300 characters" });
        }

        [HttpPost]
        [Route("Language/Delete")]
        [Authorize]

        public async Task<IActionResult> DeleteBook([FromBody] DeleteLanguagePayload language)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            if (language == null)
            {
                return BadRequest(new { Message = "No data has been passed" });
            }

            try
            {
                await _languageService.DeleteLanguage(language.LanguageId);
                return Ok(new { Message = "Language Deleted successfully!" });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }


        [HttpPost]
        [Route("Language/Edit")]
        [Authorize]
        public async Task<IActionResult> EditGenre([FromBody]LanguageViewModel language)
        {

            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                try
                {
                    await _languageService.EditLanguage(language);
                    return Ok(new { Message = "Language Successfully edited" });
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
                    return BadRequest(new { Message = ex.Message });
                }

            }

            return BadRequest(new { Message = "Title should not be null or exceed 300 characters" });
        }



        [HttpGet]
        [Route("Language/LanguageView/{languageId}")]
        [Authorize]
        public async Task<IActionResult> GetBooksByGenre(string languageId)
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");
            try
            {
                // Get the books by language
                List<BookViewModel> retrievedBooksByLanguage = await _languageService.GetBooksByLanguage(languageId);
                LanguageViewModel retrievedLanguageById = await _languageService.GetLanguageByName(languageId);

                // Create a paginated list from the regular list
                const int pageSize = 10;
                int pageIndex = 1;

                // Create a PaginatedList using the constructor
                PaginatedList<BookViewModel> paginatedBooks =
                    new PaginatedList<BookViewModel>(
                        retrievedBooksByLanguage,
                        retrievedBooksByLanguage.Count,
                        pageIndex,
                        pageSize);

                // Get all genres to avoid null reference exception in the view
                var bookGenreService = HttpContext.RequestServices.GetService<IBookGenreService>();
                if (bookGenreService != null)
                {
                    ViewData["AllGenres"] = await bookGenreService.GetAllGenreList();
                }
                else
                {
                    // Provide an empty list if the service is unavailable
                    ViewData["AllGenres"] = new List<BookGenreViewModel>();
                }

                // Set other ViewData properties that the view expects
                ViewData["CurrentSearch"] = null;
                ViewData["CurrentAuthor"] = null;
                ViewData["CurrentRating"] = null;
                ViewData["CurrentFromDate"] = null;
                ViewData["CurrentToDate"] = null;
                ViewData["CurrentGenres"] = Array.Empty<string>();
                ViewData["CurrentSort"] = "title";
                ViewData["CurrentSortDescending"] = false;

                ViewBag.CurrentLanguageDetails = retrievedLanguageById;

                // Pass the paginated list to the view
                return View("~/Views/Books/ListBook.cshtml", paginatedBooks);
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
