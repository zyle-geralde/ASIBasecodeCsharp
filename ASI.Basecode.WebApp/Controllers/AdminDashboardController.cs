using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.AccessControl;
using ASI.Basecode.WebApp.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ASI.Basecode.WebApp.Controllers
{
    public class AdminDashboardController: Controller
    {
        private readonly IBookService _bookService;
        private readonly IBookRepository _bookRepository;
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IUserRepository _userRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IAccessControlInterface _accessControlInterface;

        public AdminDashboardController(IBookService bookService,
            IBookRepository bookRepository,
            IBookGenreRepository bookGenreRepository,
            IUserRepository userRepository,
            IReviewRepository reviewRepository,
            IAccessControlInterface accessControlInterface)
        {
            _bookService = bookService;
            _bookRepository = bookRepository;
            _bookGenreRepository = bookGenreRepository;
            _userRepository = userRepository;
            _reviewRepository = reviewRepository;
            _accessControlInterface = accessControlInterface;
        }

        [HttpGet]
        [Authorize]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

        public async Task<IActionResult> Index()
        {
            bool checkAdminAccess = await _accessControlInterface.CheckAdminAccess();
            if (!checkAdminAccess) return RedirectToAction("Index", "Home");

            var books = await _bookRepository.GetAllBooks() ?? new List<Book>();
            var genres = await _bookGenreRepository.GetAllGenreList() ?? new List<BookGenre>();
            var users = _userRepository.GetUsers()?.ToList()?? new List<User>();
            var reviews = await _reviewRepository.GetAllReviews() ?? new List<Review>();

            var topRatedParams = new BookQueryParams
            {
                SortOrder = "rating",
                SortDescending = true,
                PageSize = 12
            };
            var topRatedBooks = await _bookService.GetBooks(topRatedParams);

            var newlyAddedParams = new BookQueryParams
            {
                SortOrder = "uploaddate",
                SortDescending = true,
                PageSize = 12
            };
            var newlyAddedBooks = await _bookService.GetBooks(newlyAddedParams);

            var vm = new AdminDashboardViewModel
            {
                TotalBooks = books.Count,
                TotalGenres = genres.Count,
                TotalUsers = users.Count,
                TotalReviews = reviews.Count,
                TopRatedBooks = topRatedBooks ?? new List<BookViewModel>(),
                NewBooks = newlyAddedBooks ?? new List<BookViewModel>(),
                FeaturedBooks = new List<BookViewModel>()
            };
            return View(vm);

        }
    }
}
