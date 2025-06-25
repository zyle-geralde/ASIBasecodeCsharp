using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class BookRepository:IBookRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;

        public BookRepository(AsiBasecodeDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddBook(Book book)
        {
            await _dbContext.Books.AddAsync(book);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<Book>> GetBooks(BookQueryParams queryParams) 
        {
            IQueryable<Book> query = _dbContext.Books.AsNoTracking();

            // SEARCH
            if (!string.IsNullOrEmpty(queryParams.SearchTerm))
            {
                var term = queryParams.SearchTerm.Trim();
                query = query.Where(b =>
                (b.Title != null && b.Title.Contains(term) || (b.Subtitle != null && b.Subtitle.Contains(term)) || (b.Author != null && b.Author.Contains(term))));

            }

            if (!string.IsNullOrEmpty(queryParams.Author))
                query = query.Where(b => b.Author != null && b.Author.Contains(queryParams.Author.Trim()));

            if (queryParams.Rating.HasValue)
                query = query.Where(b => b.AverageRating >= queryParams.Rating.Value);

            if (queryParams.PublishedFrom.HasValue && queryParams.PublishedTo.HasValue)
                query = query.Where(b => b.PublicationDate >= queryParams.PublishedFrom.Value && b.PublicationDate <= queryParams.PublishedTo.Value);

            if (queryParams.GenreNames != null && queryParams.GenreNames.Any())
            {
                foreach (var gn in queryParams.GenreNames)
                {
                    var pattern = "%," + gn.Trim() + ",%";
                    query = query.Where(b =>
                        b.GenreList != null
                        && b.GenreList.Contains(gn));
                }
            }

            if (!string.IsNullOrEmpty(queryParams.SortOrder))
            {
                bool desc = queryParams.SortDescending;
                switch (queryParams.SortOrder.Trim().ToLower())
                {
                    case "title":
                        query = desc
                            ? query.OrderByDescending(b => b.Title)
                            : query.OrderBy(b => b.Title);
                        break;

                    case "publicationdate":
                        query = desc
                            ? query.OrderByDescending(b => b.PublicationDate)
                            : query.OrderBy(b => b.PublicationDate);
                        break;

                    case "rating":
                        query = desc
                            ? query.OrderByDescending(b => b.AverageRating)
                            : query.OrderBy(b => b.AverageRating);
                        break;
                    case "uploaddate":
                        query = desc ? query.OrderByDescending(b => b.UploadDate) : query.OrderBy(b => b.UploadDate);
                        break;
                    default:
                        query = query.OrderBy(b => b.Title);
                        break;
                }
            }


            query = query
                .Skip((queryParams.PageIndex - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize);

            return await query.ToListAsync();
        }
        public async Task<Book?> GetBookById(string bookId)
        {
            return await _dbContext.Books.FirstOrDefaultAsync(book => book.BookId == bookId);
        }

        public async Task EditBook(Book book)
        {
            Book existingBook = await GetBookById(book.BookId);

            //Transfer to Service
            existingBook.Title = book.Title;
            existingBook.Subtitle = book.Subtitle;
            existingBook.Author = book.Author;
            existingBook.Publisher = book.Publisher;
            existingBook.PublicationDate = book.PublicationDate;
            existingBook.PublicationLocation = book.PublicationLocation;
            existingBook.Language = book.Language;
            existingBook.NumberOfPages = book.NumberOfPages;
            existingBook.UpdatedDate = book.UpdatedDate;
            existingBook.SeriesName = book.SeriesName;
            existingBook.SeriesOrder = book.SeriesOrder;
            existingBook.SeriesDescription = book.SeriesDescription;
            existingBook.Description = book.Description;
            existingBook.ISBN10 = book.ISBN10;
            existingBook.ISBN13 = book.ISBN13;
            existingBook.Edition = book.Edition;
            existingBook.CoverImage = book.CoverImage;
            existingBook.BookFile = book.BookFile;
            existingBook.GenreList = book.GenreList;
            existingBook.UpdatedBy = book.UpdatedBy;//change to Updated


            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteBook(string bookId)
        {
            Book existingBook = await GetBookById(bookId);

            _dbContext.Books.Remove(existingBook);
            await _dbContext.SaveChangesAsync();
            
        }

        public async Task<List<BookGenre>> GetAllGenres()
        {
            try
            {
                return await _dbContext.BookGenres.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task calculateAverageRating(string bookId)
        {
            var reviews = _dbContext.Reviews
                .Where(r => r.BookId == bookId);

            double avg;
            if (await reviews.AnyAsync())
                avg = await reviews.AverageAsync(r => r.Rating);
            else
                avg = 0;

            var book = await _dbContext.Books.FindAsync(bookId);
            if (book != null)
            {
                book.AverageRating = (float)avg;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Book>> GetAllBooks()
        {
            try
            {
                return await _dbContext.Books.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
