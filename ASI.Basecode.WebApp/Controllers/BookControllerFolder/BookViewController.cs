using ASI.Basecode.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Http;
using System;


namespace ASI.Basecode.WebApp.Controllers.BookControllerFolder
{
    //This controller will handle actions that return views
    public class BookViewController : Controller
    {

        private readonly IBookService _bookService;
        public BookViewController(IBookService bookService)
        {
            _bookService = bookService;
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
           Book book = await _bookService.GetBookById(bookId);
           return View("~/Views/Books/EditBook.cshtml",book);
        }

        [HttpGet]
        [Route("Book/BookDetails/{bookId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBook(string bookId)
        {
            Book book = await _bookService.GetBookById(bookId);
            return View("~/Views/Books/BookDetails.cshtml", book);
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

            return View(book);
        }
    }
}
