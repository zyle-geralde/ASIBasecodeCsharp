using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class BookGenreService:IBookGenreService
    {
        private readonly IBookGenreRepository BookGenreRepository;
        public BookGenreService(IBookGenreRepository book_genre_repositry) 
        {
            BookGenreRepository = book_genre_repositry;
        }

        public async Task AddGenre(BookGenreViewModel book_genre)
        {
            if(book_genre == null)
            {
                throw new ArgumentNullException(nameof(book_genre),"Book Genre should not be null");
            }

            if (string.IsNullOrEmpty(book_genre.GenreName))
            {
                throw new ArgumentException("Genre name cannot be null or empty");
            }
            if (book_genre.GenreName.Contains(','))
            {
                throw new ArgumentException("Genre name cannot contain commas.");
            }


            bool check_user_exist = await BookGenreRepository.CheckGenreExist(book_genre.GenreName);

            if (check_user_exist)
            {
                throw new ArgumentException("Genre Name already exist");
            }
            

            var mapped_book_genre = new BookGenre 
            {
                BookGenreId = Guid.NewGuid().ToString(),
                GenreName = book_genre.GenreName,
                GenreDescription = book_genre.GenreDescription,
                GenreImageUrl = book_genre.GenreImageUrl,
                CreatedBy = "admin1",
                UpdatedBy = "admin1",
                UpdatedDate = DateTime.Now,
                UploadDate = DateTime.Now,
            };

            try
            {
                await BookGenreRepository.AddGenre(mapped_book_genre);
            }
            catch (Exception ex) {
                throw new ApplicationException($"Failed to add book genre", ex);
            }
        }

        public async Task<List<BookGenreViewModel>> GetAllGenreList()
        {
            
                List<BookGenre> book_genre_list = await BookGenreRepository.GetAllGenreList();


                if(book_genre_list == null || !book_genre_list.Any())
                {
                    return new List<BookGenreViewModel>();
                }

                //Mapping
                List<BookGenreViewModel> view_model_list = book_genre_list.Select(book_genre_element => new BookGenreViewModel
                {
                    BookGenreId = book_genre_element.BookGenreId,
                    GenreName = book_genre_element.GenreName,
                    GenreDescription = book_genre_element.GenreDescription,
                    GenreImageUrl = book_genre_element.GenreImageUrl,
                    CreatedBy = book_genre_element.CreatedBy,
                    UpdatedDate = book_genre_element.UpdatedDate,
                    UploadDate = book_genre_element.UploadDate,
                    UpdatedBy = book_genre_element.UpdatedBy
                }).ToList();


                return view_model_list;

        }

        public async Task<BookGenreViewModel> GetBookGenreById(string genre_id)
        {

            if (string.IsNullOrEmpty(genre_id))
            {
                throw new ArgumentNullException(nameof(genre_id), "Book Genre should not be null");
            }

            try
            {
                BookGenre retreived_genre = await BookGenreRepository.GetBookGenreById(genre_id);

                BookGenreViewModel mapped_genre = new BookGenreViewModel
                {
                    BookGenreId = retreived_genre.BookGenreId,
                    GenreName = retreived_genre.GenreName,
                    GenreDescription = retreived_genre.GenreDescription,
                    GenreImageUrl = retreived_genre.GenreImageUrl,
                };

                return mapped_genre;
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Failed to retrieve book genre by id.", ex);
            }
        }

        public async Task EditGenre(BookGenreViewModel book_genre)
        {
            if(book_genre == null)
            {
                throw new ArgumentNullException(nameof(book_genre), "Book Genre should not be null");
            }


            try
            {
                BookGenre existing_genre = await BookGenreRepository.GetBookGenreById(book_genre.BookGenreId);
                
                bool check_user_exist = await BookGenreRepository.CheckGenreExist(book_genre.GenreName);

                if (check_user_exist && existing_genre.GenreName != book_genre.GenreName)
                {
                    throw new ArgumentException("Genre Name already exist");
                }

                existing_genre.GenreName = book_genre.GenreName;
                existing_genre.GenreDescription = book_genre.GenreDescription;
                existing_genre.GenreImageUrl = book_genre.GenreImageUrl;
                existing_genre.UpdatedDate = DateTime.UtcNow;

                await BookGenreRepository.EditGenre();
            }
            catch(ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Failed to edit book genre by id.", ex);
            }
        }

        public async Task DeleteGenre(string genre_id)
        {

            try
            {
                await BookGenreRepository.DeleteGenre(genre_id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to Delete book: {ex.Message}", ex);
            }

        }

        public async Task<List<BookViewModel>>GetBooksByGenre(string genre_name)
        {
            try
            {
                var all_books = await BookGenreRepository.GetBooksByGenre();
                var filtered_books = all_books.Where(book => book.GenreList != null && book.GenreList.ToLower().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Any(g => g.Trim() == genre_name.ToLower())).ToList();

                var book_view_models = filtered_books.Select(bookEntity => new BookViewModel
                {
                    BookId = bookEntity.BookId,
                    Title = bookEntity.Title,
                    Subtitle = bookEntity.Subtitle,
                    Description = bookEntity.Description,
                    NumberOfPages = bookEntity.NumberOfPages,
                    Language = bookEntity.Language,
                    SeriesName = bookEntity.SeriesName,
                    SeriesDescription = bookEntity.SeriesDescription,
                    SeriesOrder = bookEntity.SeriesOrder,
                    GenreList = bookEntity.GenreList,

                    // Firebase Storage URLs are directly mapped
                    CoverImageUrl = bookEntity.CoverImage,
                    BookFileUrl = bookEntity.BookFile,

                    // Parse dates from string (assuming "yyyy-MM-dd" or similar from frontend)
                    UpdatedDate = bookEntity.UpdatedDate,
                    PublicationDate = bookEntity.PublicationDate,


                    // Handle comma-separated strings
                    Publisher = bookEntity.Publisher, // Store as string
                    PublicationLocation = bookEntity.PublicationLocation, // Store as string
                    Author = bookEntity.Author, // Store as string
                    ISBN10 = bookEntity.ISBN10,
                    ISBN13 = bookEntity.ISBN13,
                    Edition = bookEntity.Edition,
                    CreatedBy = "admin1",
                    UpdatedBy = "Logged Admin"
                }).ToList();

                return book_view_models;

            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to Get books by genre: {ex.Message}", ex);
            }
        }

        public async Task<BookGenreViewModel> GetBookGenreByName(string genre_name)
        {

            if (string.IsNullOrEmpty(genre_name))
            {
                throw new ArgumentNullException(nameof(genre_name), "Book Genre Name should not be null");
            }

            try
            {
                BookGenre retreived_genre = await BookGenreRepository.GetBookGenreByName(genre_name);

                BookGenreViewModel mapped_genre = new BookGenreViewModel
                {
                    BookGenreId = retreived_genre.BookGenreId,
                    GenreName = retreived_genre.GenreName,
                    GenreDescription = retreived_genre.GenreDescription,
                    GenreImageUrl = retreived_genre.GenreImageUrl,
                    UpdatedDate = retreived_genre.UpdatedDate,
                    UpdatedBy = retreived_genre.UpdatedBy,
                    CreatedBy = retreived_genre.CreatedBy
                };

                return mapped_genre;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve book genre by name.", ex);
            }
        }
    }
}
