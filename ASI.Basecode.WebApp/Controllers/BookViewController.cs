﻿using ASI.Basecode.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASI.Basecode.Services.Interfaces;

namespace ASI.Basecode.WebApp.Controllers
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
            return View("~/Views/Books/ListBook.cshtml",books); 
        }
    }
}
