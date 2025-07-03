using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.QueryParams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IBookGenreRepository
    {
        Task AddGenre(BookGenre book_genre);

        Task<bool> CheckGenreExist(string genre_name);

        Task<List<BookGenre>> GetAllGenreList();

        Task<BookGenre> GetBookGenreById(string genre_id);

        Task EditGenre();

        Task DeleteGenre(string genre_id);

        Task<List<Book>> GetBooksByGenre(); 

        Task<BookGenre> GetBookGenreByName(string genre_name);

        Task<PaginatedList<BookGenre>> GetGenreQueried(GenreQueryParams? queryParams = null);

    }
}
