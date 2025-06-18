using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class BookGenreRepository:IBookGenreRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;
        public BookGenreRepository(AsiBasecodeDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddGenre(BookGenre book_genre)
        {
            if (book_genre == null)
            {
                throw new ArgumentNullException(nameof(book_genre),"Book genre cannot be Null");
            }

            try
            {
                await _dbContext.BookGenres.AddAsync(book_genre);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
