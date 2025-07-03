using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.CodeDom;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.QueryParams;
//using System.Data.Entity;

namespace ASI.Basecode.Data.Repositories
{
    public class BookGenreRepository:BaseRepository,IBookGenreRepository
    {
        private readonly AsiBasecodeDBContext DbContext;
        public BookGenreRepository(AsiBasecodeDBContext db_context, IUnitOfWork unitOfWork):base(unitOfWork)
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
        public async Task<List<BookGenre>> GetAllGenreList()
        {
            try
            {
                return await DbContext.BookGenres.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task EditGenre()
        {
            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch(Exception ex){
                throw;
            }
        }

        public async Task DeleteGenre(string genre_id)
        {
            BookGenre existing_genre = await GetBookGenreById(genre_id);

            DbContext.BookGenres.Remove(existing_genre);
            await DbContext.SaveChangesAsync();

        }
        public async Task<List<Book>> GetBooksByGenre()
        {
            try
            {
                // 
                return await DbContext.Books.ToListAsync();
            }
            catch(Exception ex)
            {
               throw;
            }
        }

        public async Task<BookGenre> GetBookGenreByName(string genre_name)
        {
            try
            {
                return await DbContext.BookGenres.FirstOrDefaultAsync(genre => genre.BookGenreId == genre_name);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<PaginatedList<BookGenre>> GetGenreQueried(GenreQueryParams? queryParams = null)
        {

            queryParams ??= new GenreQueryParams();

            IQueryable<BookGenre> query = this.GetDbSet<BookGenre>().AsNoTracking();

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(queryParams.SearchTerm))
            {
                var term = queryParams.SearchTerm.Trim();
                query = query.Where(b =>
                    (b.GenreName != null && b.GenreName.Contains(term)));
            }

            if (!string.IsNullOrEmpty(queryParams.SortOrder))
            {
                bool desc = queryParams.SortDescending;
                switch (queryParams.SortOrder.ToLower())
                {
                    case "Name":
                        query = desc
                           ? query.OrderByDescending(b => b.GenreName)
                           : query.OrderBy(b => b.GenreName);
                        break;
                    case "CreatedTime":
                        query = desc
                        ? query.OrderByDescending(b => b.UploadDate)
                        : query.OrderBy(b => b.UploadDate);
                        break;
                    default:
                        query = desc
                        ? query.OrderByDescending(b => b.GenreName)
                        : query.OrderBy(u => u.GenreName);
                        break;
                }
            }

            return await GetPaged(query, queryParams.PageIndex, queryParams.PageSize);

        }
    }
}
