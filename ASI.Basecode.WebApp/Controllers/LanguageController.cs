using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using ASI.Basecode.Services.ServiceModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using ASI.Basecode.WebApp.Payload.LanguagePayload;
using ASI.Basecode.WebApp.Payload.BooksPayload;

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
        public async Task<IActionResult> GenreList()
        {
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
        [AllowAnonymous]
        public async Task<IActionResult> AddLanguage([FromBody]LanguageViewModel language)
        {
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
        [AllowAnonymous]

        public async Task<IActionResult> DeleteBook([FromBody] DeleteLanguagePayload language)
        {
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
        [AllowAnonymous]
        public async Task<IActionResult> EditGenre([FromBody]LanguageViewModel language)
        {

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
        [AllowAnonymous]

        public async Task<IActionResult> GetBooksByGenre(string languageId)
        {
            try
            {
                //Change this during code cleaning
                List<BookViewModel> retreived_books_by_language = await _languageService.GetBooksByLanguage(languageId);
                LanguageViewModel retreived_language_by_Id = await _languageService.GetLanguageByName(languageId);

                ViewBag.CurrentLanguageDetails = retreived_language_by_Id;

                return View("~/Views/Books/ListBook.cshtml", retreived_books_by_language);
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
