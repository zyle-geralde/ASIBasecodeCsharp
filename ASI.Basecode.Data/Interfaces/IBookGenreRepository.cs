using ASI.Basecode.Data.Models;
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

        Task <List<BookGenre>> 
    }
}
