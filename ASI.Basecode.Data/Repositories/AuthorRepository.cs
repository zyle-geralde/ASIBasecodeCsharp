using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class AuthorRepository:IAuthorRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;

        public AuthorRepository(AsiBasecodeDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAuthor(Author author)
        {
            try
            {
                await _dbContext.Authors.AddAsync(author);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> CheckAuthorExist(string author_name)
        {
            return await _dbContext.Authors.AnyAsync(author_name_param => author_name_param.AuthorName.ToLower() == author_name.ToLower());
        }

        public async Task<List<Author>> GetAllAuthorList()
        {
            try
            {
                return await _dbContext.Authors.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
