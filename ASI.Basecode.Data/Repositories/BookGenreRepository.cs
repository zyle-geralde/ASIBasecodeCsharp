using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System;
using System.CodeDom;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> CheckGenreExist(string genre_name)
        {
            return await DbContext.BookGenres.AnyAsync(genre => genre.GenreName.ToLower() == genre_name.ToLower());
        }
        public async Task<List<BookGenre>> GetAllGenres()
        {
            try
            {
                return await DbContext.BookGenres.ToListAsync();
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<BookGenre> GetBookGenreById(string genre_id)
        {
            try
            {
                return await DbContext.BookGenres.FirstOrDefaultAsync(genre => genre.BookGenreId == genre_id);
            }
            catch(Exception ex)
            {
                throw;
            }
            
        }
    }
}
