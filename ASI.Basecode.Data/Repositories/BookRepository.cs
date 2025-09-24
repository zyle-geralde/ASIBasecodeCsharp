using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.QueryParams;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class BookRepository : BaseRepository, IBookRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;

        public BookRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        public async Task AddBook(Book book)
        {
            await _dbContext.Books.AddAsync(book);
            await _dbContext.SaveChangesAsync();
        }
        //public async Task<List<Book>> GetBooks(BookQueryParams? queryParams = null)
        public async Task<PaginatedList<Book>> GetBooks(BookQueryParams? queryParams = null)
        {
            queryParams ??= new BookQueryParams();

            IQueryable<Book> query = _dbContext.Books.AsNoTracking();

            if (queryParams.IsFeatured.HasValue)
            {
                query = query.Where(b => b.IsFeatured == queryParams.IsFeatured.Value);
            }

            // SEARCH
            if (!string.IsNullOrEmpty(queryParams.SearchTerm))
            {
                var term = queryParams.SearchTerm.Trim();
                var authorTerm = queryParams.SearchAuhtor?.Trim() ?? "";
                query = query.Where(b =>
                (b.Title != null && b.Title.Contains(term) || (b.Author != null && authorTerm.Trim().Contains(b.Author))));

            }

            if (!string.IsNullOrEmpty(queryParams.Author))
                //query = query.Where(b => b.Author != null && b.Author.Contains(queryParams.Author.Trim()));
                query = query.Where(b => b.Author != null && queryParams.Author.Trim().Contains(b.Author));

            if (queryParams.Rating.HasValue)
                query = query.Where(b => b.AverageRating >= queryParams.Rating.Value);

            if (queryParams.PublishedFrom.HasValue && queryParams.PublishedTo.HasValue)
                query = query.Where(b => b.PublicationDate >= queryParams.PublishedFrom.Value && b.PublicationDate <= queryParams.PublishedTo.Value);
            if (queryParams.IsFeatured.HasValue)
                query = query.Where(b => b.IsFeatured == queryParams.IsFeatured);
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
            if (!string.IsNullOrEmpty(queryParams.Language))
            {
                query = query.Where(b => b.Language == queryParams.Language);
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
                        query = desc
                            ? query.OrderByDescending(b => b.UploadDate)
                            : query.OrderBy(b => b.UploadDate);
                        break;

                    case "updateddate":
                        query = desc
                            ? query.OrderByDescending(b => b.UpdatedDate)
                            : query.OrderBy(b => b.UpdatedDate);
                        break;
                    case "reviewcount":
                        query = desc
                            ? query.OrderByDescending(b => b.ReviewCount)
                            : query.OrderBy(b => b.ReviewCount);
                        break;
                    default:
                        query = query.OrderBy(b => b.Title);
                        break;
                }
            }

            return await GetPaged(query, queryParams.PageIndex, queryParams.PageSize);
        }

        public async Task<Book?> GetBookById(string bookId)
        {
            return await _dbContext.Books.FirstOrDefaultAsync(book => book.BookId == bookId);
        }

        public async Task EditBook(Book book)
        {
            Book existingBook = await GetBookById(book.BookId);


            try
            {


                bool isBookNameAndAuthorExist = await CheckBookNameAndAuthorExist(book.Author.Trim(), book.Title.Trim().ToLower());

                if (isBookNameAndAuthorExist && (book.Author.Trim() != existingBook.Author.Trim() || book.Title.Trim().ToLower() != existingBook.Title.Trim().ToLower()))
                {
                    throw new ApplicationException("Book Title and Author Exist.");
                }

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
                existingBook.IsFeatured = book.IsFeatured;

                await _dbContext.SaveChangesAsync();
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }



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
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Language>> GetAllLanguage()
        {
            try
            {
                return await _dbContext.Languages.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task GetReviewCount(string bookId)
        {
            var book = await _dbContext.Books.FindAsync(bookId);
            if (book != null)
            {
                book.ReviewCount = await _dbContext.Reviews.CountAsync(r => r.BookId == bookId);
                await _dbContext.SaveChangesAsync();
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
            var roundedAvg = Math.Round(avg, 2);

            var book = await _dbContext.Books.FindAsync(bookId);
            if (book != null)
            {
                book.AverageRating = (float)roundedAvg;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Book>> GetAllBooks()
        {
            try
            {
                return await _dbContext.Books.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Author>> GetAllAuthor()
        {
            try
            {
                return await _dbContext.Authors.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CheckBookNameAndAuthorExist(string author, string bookName)
        {
            // Use AsNoTracking for read-only queries
            return await _dbContext.Books.AsNoTracking().AnyAsync(b => b.Author != null && b.Title != null && b.Author.ToLower() == author && b.Title.ToLower() == bookName);
        }


    }
}
