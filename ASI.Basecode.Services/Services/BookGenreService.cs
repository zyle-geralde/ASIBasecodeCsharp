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
        public BookGenreService(IBookGenreRepository book_genre_repositry) {
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


            bool check_user_exist = await BookGenreRepository.CheckGenreExist(book_genre.GenreName);

            if (check_user_exist)
            {
                throw new ArgumentException("Genre Name already exist");
            }
            

            var mapped_book_genre = new BookGenre {
                BookGenreId = Guid.NewGuid().ToString(),
                GenreName = book_genre.GenreName,
                GenreDescription = book_genre.GenreDescription,
                GenreImageUrl = book_genre.GenreImageUrl,
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
