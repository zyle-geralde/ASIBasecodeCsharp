using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IBookRepository
    {
        Task AddBook(Book book);
        //Task<List<Book>> GetBooks(BookQueryParams? queryParams = null);
        Task<PaginatedList<Book>> GetBooks(BookQueryParams? queryParams=null);
        Task<Book?> GetBookById(string bookId);

        Task EditBook(Book book);

        Task DeleteBook(string bookId);

        Task<List<BookGenre>> GetAllGenres();
        Task calculateAverageRating(string bookId);

        Task<List<Book>> GetAllBooks();

        Task<List<Language>> GetAllLanguage();

        Task<List<Author>> GetAllAuthor();

        Task<bool> CheckBookNameAndAuthorExist(string author, string bookName);

        Task GetReviewCount(string bookId);

    }
}
