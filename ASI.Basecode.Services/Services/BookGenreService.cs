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
        private readonly BookGenreRepository BookGenreRepository;
        public BookGenreService(BookGenreRepository book_genre_repositry) {
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
            if (string.IsNullOrEmpty(book_genre.GenreDescription))
            {
                throw new ArgumentException("Genre Desription cannot be null or empty");
            }
            var mapped_book_genre = new BookGenre {
                BookGenreId = Guid.NewGuid().ToString(),
                GenreName = book_genre.GenreName,
                GenreDescription = book_genre.GenreDescription,
                AdminId = "admin1",
                UpdatedByAdminId = "admin1",
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
    }
}
