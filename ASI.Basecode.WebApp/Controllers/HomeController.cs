using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.AccessControl;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization; add this to allow anonymous

namespace ASI.Basecode.WebApp.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    public class HomeController : ControllerBase<HomeController>
    {
        private readonly IBookService _bookService;
        private readonly IBookGenreService _bookGenreService;
        private readonly IAccessControlInterface _accessControlInterface;
        private readonly IAuthorService _authorService;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>
        public HomeController(IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IBookGenreService bookGenreService,
                              IBookService bookService,
                              IAccessControlInterface accessControlInterface,
                              IAuthorService authorService,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _bookService = bookService;
            _bookGenreService = bookGenreService;
            _authorService = authorService;
            this._accessControlInterface = accessControlInterface;
        }

        /// <summary>
        /// Returns Home View.
        /// </summary>
        /// <returns> Home View </returns>
        //[AllowAnonymous] //This is to bypass authentication. Ex. if you want to access this route without loging in
        [HttpGet]
        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Index()
        {
            bool checkUserAccess = await _accessControlInterface.CheckUserAccess();
            if (!checkUserAccess) return RedirectToAction("Index", "AdminDashboard");

            var allGenres = await _bookGenreService.GetAllGenreList();
            var allAuthor = await _authorService.GetAllAuthorList();

            var topRatedParams = new BookQueryParams
            {
                SortOrder = "rating",
                SortDescending = true,
                PageSize = 5
            };
            var topRatedBooks = await _bookService.GetBooks(topRatedParams);

            var newlyAddedParams = new BookQueryParams
            {
                SortOrder = "uploaddate",
                SortDescending = true,
                PageSize = 5
            };
            var newlyAddedBooks = await _bookService.GetBooks(newlyAddedParams);

            var featuredBooksParams = new BookQueryParams
            {
                IsFeatured = true,
                PageSize = 5
            };
            var featuredBooks = await _bookService.GetBooks(featuredBooksParams);

            var vm = new HomeViewModel
            {
                Genres = allGenres,
                TopRatedBooks = topRatedBooks,
                NewBooks = newlyAddedBooks,
                FeaturedBooks = featuredBooks,
                Authors = allAuthor
            };
            return View(vm);
        }
    }
}
