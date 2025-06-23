using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
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
            //Change to mapper
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
                GenreList = request.GenreList,

                // Firebase Storage URLs are directly mapped
                CoverImage = request.CoverImageUrl,
                BookFile = request.BookFileUrl,

                // Parse dates from string (assuming "yyyy-MM-dd" or similar from frontend)
                UploadDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                PublicationDate = request.PublicationDate,

                // Handle comma-separated strings
                Publisher = request.Publisher, // Store as string
                PublicationLocation = request.PublicationLocation, // Store as string
                Author = request.Author, // Store as string
                ISBN10 = request.ISBN10,
                ISBN13 = request.ISBN13,
                Edition = request.Edition,
                CreatedBy = "admin1"
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

        public async Task<BookViewModel?> GetBookById(string bookId)
        {
            Book requestBook= await _bookRepository.GetBookById(bookId);
            var book = new BookViewModel
            {
                BookId = requestBook.BookId,
                Title = requestBook.Title,
                Subtitle = requestBook.Subtitle,
                Description = requestBook.Description,
                NumberOfPages = requestBook.NumberOfPages,
                Language = requestBook.Language,
                SeriesName = requestBook.SeriesName,
                SeriesDescription = requestBook.SeriesDescription,
                SeriesOrder = requestBook.SeriesOrder,
                GenreList= requestBook.GenreList,
                AverageRating =requestBook.AverageRating,

                // Firebase Storage URLs are directly mapped
                CoverImageUrl = requestBook.CoverImage,
                BookFileUrl = requestBook.BookFile,

                // Parse dates from string (assuming "yyyy-MM-dd" or similar from frontend)
                UpdatedDate = requestBook.UpdatedDate,
                PublicationDate = requestBook.PublicationDate,


                // Handle comma-separated strings
                Publisher = requestBook.Publisher, // Store as string
                PublicationLocation = requestBook.PublicationLocation, // Store as string
                Author = requestBook.Author, // Store as string
                ISBN10 = requestBook.ISBN10,
                ISBN13 = requestBook.ISBN13,
                Edition = requestBook.Edition,
                CreatedBy = "admin1",
                UpdatedBy = "Logged Admin",
            };
            //return await _bookRepository.GetBookById(bookId);
            return book;
        }

        public async Task EditBook(BookViewModel request)
        {
            //Change to mapper
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
                GenreList = request.GenreList,

                // Firebase Storage URLs are directly mapped
                CoverImage = request.CoverImageUrl,
                BookFile = request.BookFileUrl,

                // Parse dates from string (assuming "yyyy-MM-dd" or similar from frontend)
                UpdatedDate = DateTime.UtcNow,
                PublicationDate = request.PublicationDate,

                // Handle comma-separated strings
                Publisher = request.Publisher, // Store as string
                PublicationLocation = request.PublicationLocation, // Store as string
                Author = request.Author, // Store as string
                ISBN10 = request.ISBN10,
                ISBN13 = request.ISBN13,
                Edition = request.Edition,
                CreatedBy = "admin1",
                UpdatedBy = "Logged Admin"
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

        public async Task<List<string>> GetAllGenres()
        {
            try
            {
                List<BookGenre> book_genre_list = await _bookRepository.GetAllGenres();


                if (book_genre_list == null || !book_genre_list.Any())
                {
                    return new List<string>();
                }

                //Mapping
                List<string> view_model_list = book_genre_list.Select(book_genre_element => book_genre_element.GenreName+','+ book_genre_element.BookGenreId).ToList();

                return view_model_list;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve book genres.", ex);
            }
        }

    }
}
