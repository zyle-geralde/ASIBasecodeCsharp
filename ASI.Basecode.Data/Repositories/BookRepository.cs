using System;
using System.Collections.Generic;
using System.Linq;
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

    }
}
