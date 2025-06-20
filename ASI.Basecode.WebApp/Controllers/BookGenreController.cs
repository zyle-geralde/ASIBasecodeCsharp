﻿using ASI.Basecode.Data.Models;
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
                List<BookGenreViewModel> book_genre_list = await BookGenreService.GetAllGenreList();

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
                List<BookViewModel> retreived_books_by_genre = await BookGenreService.GetBooksByGenre(genre_name);
               BookGenreViewModel retreived_genre_by_genre_name = await BookGenreService.GetBookGenreByName(genre_name);

                ViewBag.CurrentGenreDetails = retreived_genre_by_genre_name;

                return View("~/Views/BookGenres/GenreView.cshtml", retreived_books_by_genre);
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
