using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Globalization;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.WebApp.Models.Book;
using Microsoft.AspNetCore.Authorization;

namespace ASI.Basecode.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {

        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpPost("add")] // Defines a POST endpoint for adding a book
        // You would typically add authorization here, e.g., [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public async Task<IActionResult> AddBook([FromBody] CreateBookRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            // Map DTO to actual Book model
            var book = new Book
            {
                BookId = Guid.NewGuid().ToString(),
                Title = request.Title,
                Subtitle = request.Subtitle,
                Description = request.Description,
                NumberOfPages = request.NumberOfPages,
                Language = request.Language,
                SeriesName = request.SeriesName,
                SeriesDescription = request.SeriesDescription,
                AverageRating = 0,
                Likes = 0,

                // Firebase Storage URLs are directly mapped
                CoverImage = request.CoverImageUrl,
                BookFile = request.BookFileUrl,

                // Parse dates from string (assuming "yyyy-MM-dd" or similar from frontend)
                UploadDate = !string.IsNullOrWhiteSpace(request.UploadDate)
                             ? DateTime.ParseExact(request.UploadDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                             : (DateTime?)null,
                PublicationDate = !string.IsNullOrWhiteSpace(request.PublicationDate)
                                  ? DateTime.ParseExact(request.PublicationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                                  : (DateTime?)null,

                // Handle comma-separated strings for lists
                Publisher = request.Publisher, // Store as string if no ValueConverter
                PublicationLocation = request.PublicationLocation, // Store as string
                Author = request.Author // Store as string
            };

            try
            {
                await _bookService.AddBook(book);
                return Ok(new { Message = "Book added successfully!", BookId = book.BookId });
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using ILogger)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
