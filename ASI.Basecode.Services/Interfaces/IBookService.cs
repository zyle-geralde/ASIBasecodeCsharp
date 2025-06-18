using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IBookService
    {
        Task AddBook(BookViewModel book);
        Task<List<Book>> GetAllBooks();
        Task<BookViewModel?> GetBookById(string bookId);

        Task EditBook(BookViewModel request);

        Task DeletBook(string bookId);
    }
}
