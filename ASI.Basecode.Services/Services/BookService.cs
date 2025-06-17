using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task AddBook(BookViewModel request)
        {
            // Here you can add any business logic or validation before saving the book to the database.
            //example:if (string.IsNullOrWhiteSpace(book.Title)) throw new ArgumentException("Book title cannot be empty.");

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
                SeriesOrder = request.SeriesOrder,
                AverageRating = 0,
                Likes = 0,

                // Firebase Storage URLs are directly mapped
                CoverImage = request.CoverImageUrl,
                BookFile = request.BookFileUrl,

                // Parse dates from string (assuming "yyyy-MM-dd" or similar from frontend)
                UploadDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                PublicationDate = !string.IsNullOrWhiteSpace(request.PublicationDate)
                                  ? DateTime.ParseExact(request.PublicationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                                  : (DateTime?)null,

                // Handle comma-separated strings
                Publisher = request.Publisher, // Store as string
                PublicationLocation = request.PublicationLocation, // Store as string
                Author = request.Author, // Store as string
                ISBN10 = request.ISBN10,
                ISBN13 = request.ISBN13,
                Edition = request.Edition,
                AdminId = "admin1"
            };


            try
            {
                await _bookRepository.AddBook(book); 
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to add book: {ex.Message}", ex);
            }
        }

        public async Task<List<Book>> GetAllBooks()
        {
            return await _bookRepository.GetAllBooks();
        }

        public async Task<Book?> GetBookById(string bookId)
        {
            return await _bookRepository.GetBookById(bookId);
        }

        public async Task EditBook(BookViewModel request)
        {
            var book = new Book
            {
                BookId = request.BookId,
                Title = request.Title,
                Subtitle = request.Subtitle,
                Description = request.Description,
                NumberOfPages = request.NumberOfPages,
                Language = request.Language,
                SeriesName = request.SeriesName,
                SeriesDescription = request.SeriesDescription,
                SeriesOrder = request.SeriesOrder,

                // Firebase Storage URLs are directly mapped
                CoverImage = request.CoverImageUrl,
                BookFile = request.BookFileUrl,

                // Parse dates from string (assuming "yyyy-MM-dd" or similar from frontend)
                UpdatedDate = DateTime.UtcNow,
                PublicationDate = !string.IsNullOrWhiteSpace(request.PublicationDate)
                                 ? DateTime.ParseExact(request.PublicationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                                 : (DateTime?)null,

                // Handle comma-separated strings
                Publisher = request.Publisher, // Store as string
                PublicationLocation = request.PublicationLocation, // Store as string
                Author = request.Author, // Store as string
                ISBN10 = request.ISBN10,
                ISBN13 = request.ISBN13,
                Edition = request.Edition,
                AdminId = "admin1",
                UpdatedByAdminId = "Logged Admin"
            };


            try
            {
                await _bookRepository.EditBook(book);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to Edit book: {ex.Message}", ex);
            }
        }

        public async Task DeletBook(string bookId)
        {

            try
            {
                await _bookRepository.DeleteBook(bookId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to Delete book: {ex.Message}", ex);
            }
            
        }

    }
}
