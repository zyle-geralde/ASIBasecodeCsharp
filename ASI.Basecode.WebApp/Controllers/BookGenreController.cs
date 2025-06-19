using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
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
                List<BookGenreViewModel> book_genre_list = await BookGenreService.GetAllGenres();

                return View("~/Views/BookGenres/BookGenreList.cshtml", book_genre_list);
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
    }
}
