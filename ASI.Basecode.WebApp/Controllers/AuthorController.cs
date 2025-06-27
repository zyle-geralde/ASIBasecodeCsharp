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

        [HttpGet]
        [Route("Author/EditAuthor")]
        [AllowAnonymous]
        public IActionResult EditAuthor()
        {
            return View("~/Views/Author/EditAuthor.cshtml");
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

            return View("~/Views/Author/EditAuthor.cshtml");
        }
    }
}
