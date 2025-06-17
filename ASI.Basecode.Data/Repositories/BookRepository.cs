using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;

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
        public async Task<List<Book>> GetAllBooks() 
        {
            return await _dbContext.Books.ToListAsync();
        }
        public async Task<Book?> GetBookById(string bookId)
        {
            return await _dbContext.Books.FirstOrDefaultAsync(book => book.BookId == bookId);
        }

        public async Task EditBook(Book book)
        {
            Book existingBook = await GetBookById(book.BookId);


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
            existingBook.UpdatedByAdminId = book.UpdatedByAdminId;


            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteBook(string bookId)
        {
            Book existingBook = await GetBookById(bookId);

            _dbContext.Books.Remove(existingBook);
            await _dbContext.SaveChangesAsync();
            
        }

    }
}
