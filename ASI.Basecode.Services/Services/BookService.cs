using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task AddBook(Book book)
        {
            // Here you can add any business logic or validation before saving the book to the database.
            //example:if (string.IsNullOrWhiteSpace(book.Title)) throw new ArgumentException("Book title cannot be empty.");

            await _bookRepository.AddBook(book);
        }

    }
}
