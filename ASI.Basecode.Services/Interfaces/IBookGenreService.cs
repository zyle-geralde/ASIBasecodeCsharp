using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IBookGenreService
    {
        Task AddGenre(BookGenreViewModel book_genre);
        Task<List<BookGenreViewModel>> GetAllGenreList(); 

        Task<BookGenreViewModel> GetBookGenreById(string genre_id);
        Task EditGenre(BookGenreViewModel book_genre);

        Task DeleteGenre(string genre_id);
    }
}
