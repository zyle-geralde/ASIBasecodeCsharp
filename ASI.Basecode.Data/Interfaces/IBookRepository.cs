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
        Task<List<Book>> GetAllBooks();

        Task<Book?> GetBookById(string bookId);


    }
}
