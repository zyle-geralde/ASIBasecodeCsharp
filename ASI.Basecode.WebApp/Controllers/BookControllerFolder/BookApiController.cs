using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Controllers.BookControllerFolder
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookApiController : ControllerBase
    {

        private readonly IBookService _bookService;

        public BookApiController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpPost("add")] // Defines a POST endpoint for adding a book
        // You would typically add authorization here, e.g., [Authorize(Roles = "Admin")]
        [AllowAnonymous]//Added to bypass authorization. No need to LogIn or Sign Up
        public async Task<IActionResult> AddBook([FromBody] BookViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            try
            {
                // Call the service method, which now encapsulates the mapping and database operation
                await _bookService.AddBook(request);
                return Ok(new { Message = "Book added successfully!" });
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

        [HttpPost("edit")]
        [AllowAnonymous]
        public async Task<IActionResult> EditBook([FromBody] BookViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }
            request.UpdatedDate = DateTime.UtcNow;
            request.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                // Call the service method, which now encapsulates the mapping and database operation
                await _bookService.EditBook(request);
                return Ok(new { Message = "Book Edited successfully!" });
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

        [HttpPost("delete")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteBook(string bookId)
        {
            try
            {
                await _bookService.DeletBook(bookId);
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
