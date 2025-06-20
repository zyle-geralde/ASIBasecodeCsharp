﻿using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Payload.BooksPayload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public BookViewController(IBookService bookService, IReviewService reviewService)
        {
            _bookService = bookService;
            _reviewService = reviewService;
        }


        [HttpGet]
        [Route("Book/AddBook")]
        [AllowAnonymous]// Bypass authorization. No need to Log In 
        public IActionResult AddBook()
        {
            // This will look for a view at /Views/Books/AddBook.cshtml
            return View("~/Views/Books/AddBook.cshtml");
        }

        [HttpGet]
        [Route("Book/GetGenre")]
        [AllowAnonymous]
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
        [Route("Book/ListBook")]
        [AllowAnonymous]
        public async Task<IActionResult> ListBook()
        {
            List<Book> books = await _bookService.GetAllBooks();
            return View("~/Views/Books/ListBook.cshtml", books);
        }

        [HttpGet]
        [Route("Book/EditBook/{bookId}")]
        [AllowAnonymous]
        public async Task<IActionResult> EditBook(string bookId)
        {
           BookViewModel book = await _bookService.GetBookById(bookId);
           return View("~/Views/Books/EditBook.cshtml",book);
        }

        [HttpGet]
        [Route("Book/BookDetails/{bookId}", Name="BookDetails")]
        [AllowAnonymous]
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
                BookFileUrl = book.BookFileUrl,
                SeriesName = book.SeriesName,
                SeriesOrder = book.SeriesOrder,
                SeriesDescription = book.SeriesDescription,
                AverageRating = book.AverageRating,
                Author = book.Author,
                Likes = book.Likes,
                ISBN10 = book.ISBN10,
                ISBN13 = book.ISBN13,
                Edition = book.Edition,
                HasReviewed = hasReviewed,
                Reviews = reviews
                            .Select(r => new ReviewViewModel
                            {
                                ReviewId = r.UserId,
                                Rating = r.Rating,
                                Comment = r.Comment,
                                UploadDate = r.UploadDate
                            })
                            .ToList()
            };


            return View("~/Views/Books/BookDetails.cshtml", bookDetails);
        }

        [HttpPost]
        [Route("Book/AddBook")]
        [AllowAnonymous]
        public async Task<IActionResult> AddBook(BookViewModel book)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _bookService.AddBook(book);

                    TempData["SuccessMessage"] = "Book added successfully!";
                    return RedirectToAction("ListBook"); 
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An unexpected error occurred: " + ex.Message);
                }
            }

            return View("~/Views/Books/AddBook.cshtml", book);
        }

        [HttpPost]
        [Route("Book/EditBook")]
        [AllowAnonymous]
        public async Task<IActionResult> EditBook(BookViewModel book)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _bookService.EditBook(book);
                    return Ok(new { Message = "Book Edited successfully!" });
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error updating book: {ex.Message}");
                    return StatusCode(500, $"Error updating book: {ex.Message}");
                }
            }


            //return bad request if ModelState is not valid
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { errors = errors, Message = "Validation failed." });
        }

        [HttpPost]
        [Route("Book/Delete")]
        [AllowAnonymous]

        public async Task<IActionResult> DeleteBook([FromBody] DeleteBookPayload book)
        {
            if(book == null)
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
    }
}
