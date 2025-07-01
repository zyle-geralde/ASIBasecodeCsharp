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
using ASI.Basecode.WebApp.AccessControl;

namespace ASI.Basecode.WebApp.Controllers
{
    public class LanguageController : Controller
    {

        private readonly ILanguageService _languageService;
        private readonly IAccessControlInterface _accessControlInterface;

        public LanguageController(ILanguageService languageService,IAccessControlInterface accessControlInterface)
        {
            _languageService = languageService;
            _accessControlInterface = accessControlInterface;
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
                    TempData["message"] = "Language Added successfully!";
                    TempData["showToastrAfterTableLoads"] = true;

                    return RedirectToAction(nameof(LanguageList)); // Redirect to the LanguageList action
                    // return Ok(new { Message = "Language Added successfully!" });
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
                TempData["message"] = "Language deleted successfully!";
                TempData["showToastrAfterTableLoads"] = true;

                return RedirectToAction(nameof(LanguageList));
               // return Ok(new { Message = "Language Deleted successfully!" });
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
                    TempData["message"] = "Language updated successfully!";
                    TempData["showToastrAfterTableLoads"] = true;

                    return RedirectToAction(nameof(LanguageList));
                   // return Ok(new { Message = "Language Successfully edited" });

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
