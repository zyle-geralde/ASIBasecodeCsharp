using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Services;
using System.Linq;
using ASI.Basecode.Services.Interfaces;
using Microsoft.CodeAnalysis.Host;
using ASI.Basecode.WebApp.Payload.BooksPayload;
using ASI.Basecode.WebApp.Payload.AuthorPayload;

namespace ASI.Basecode.WebApp.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IAuthorService _authorService;
        private readonly IBookService _bookService;

        public AuthorController(IAuthorService authorService, IBookService book_service)
        {
            _authorService = authorService;
            _bookService = book_service;
        }

        [HttpGet]
        [Route("Author/Index")]
        [AllowAnonymous]
        public async Task<IActionResult> GenreList()
        {
            try
            {
                List<AuthorViewModel> language_list = await _authorService.GetAllAuthorList();

                return View("~/Views/Author/Index.cshtml", language_list);
            }
            catch (ApplicationException ex)
            {

                return View("~/Views/Author/Index.cshtml", new List<LanguageViewModel>());
            }
            catch (Exception ex)
            {
                return View("~/Views/Author/Index.cshtml", new List<LanguageViewModel>());

            }
        }


        [HttpGet]
        [Route("Author/AddAuthor")] 
        [AllowAnonymous]
        public IActionResult AddAuthor()
        {
            return View("~/Views/Author/AddAuthor.cshtml");
        }

        [HttpGet]
        [Route("Author/EditAuthor/{author_id}")]
        [AllowAnonymous]
        public async Task<IActionResult> EditAuthor(string author_id)
        {
            try
            {
                AuthorViewModel retreived_author = await _authorService.GetAuthorById(author_id) ;

                return View("~/Views/Author/EditAuthor.cshtml", retreived_author);
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
        [AllowAnonymous]
        public async Task<IActionResult> AddAuthor(AuthorViewModel author)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _authorService.AddAuthor(author);
                    return Ok(new { Message ="Author Successfully added" });
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
        [AllowAnonymous]
        public async Task<IActionResult> EditAuthor(AuthorViewModel author)
        {

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
        [AllowAnonymous]

        public async Task<IActionResult> DeleteAuthor([FromBody] DeleteAuthorPayload author)
        {
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
    }
}
