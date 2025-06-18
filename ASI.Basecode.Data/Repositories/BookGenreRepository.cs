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
        private readonly AsiBasecodeDBContext DbContext;
        public BookGenreRepository(AsiBasecodeDBContext db_context)
        {
            DbContext = db_context;
        }

        public async Task AddGenre(BookGenre book_genre)
        {
            if (book_genre == null)
            {
                throw new ArgumentNullException(nameof(book_genre),"Book genre cannot be Null");
            }

            try
            {
                await DbContext.BookGenres.AddAsync(book_genre);
                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
