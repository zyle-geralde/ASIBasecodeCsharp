using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.QueryParams;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class AuthorRepository : BaseRepository, IAuthorRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;

        public AuthorRepository(AsiBasecodeDBContext dbContext,IUnitOfWork unitOfWork) : base(unitOfWork)
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
        public async Task EditAuthor()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Author> GetAuthorById(string author_id)
        {
            try
            {
                return await _dbContext.Authors.FirstOrDefaultAsync(author => author.AuthorId == author_id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task DeleteAuthor(string auhtor_id)
        {
            Author existing_author = await GetAuthorById(auhtor_id);

            _dbContext.Authors.Remove(existing_author);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Book>> GetBooksByAuthor()
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

        public async Task<string> GetAuthorByName(string author_name)
        {
            try
            {
                if (string.IsNullOrEmpty(author_name))
                {
                    return "";
                }

                var matchingAuthorIds = await _dbContext.Authors
                                                        .Where(author => author.AuthorName != null && author.AuthorName.Contains(author_name))
                                                        .Select(author => author.AuthorId)
                                                        .ToListAsync();

                return string.Join(",", matchingAuthorIds);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PaginatedList<Author>> GetAuthorQueried(AuthorQueryParams? queryParams = null)
        {

            queryParams ??= new AuthorQueryParams();

            IQueryable<Author> query = this.GetDbSet<Author>().AsNoTracking();

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(queryParams.SearchTerm))
            {
                var term = queryParams.SearchTerm.Trim();
                query = query.Where(b =>
                    (b.AuthorName != null && b.AuthorName.Contains(term)));
            }

            if (!string.IsNullOrEmpty(queryParams.SortOrder))
            {
                bool desc = queryParams.SortDescending;
                switch (queryParams.SortOrder.ToLower())
                {
                    case "name":
                        query = desc
                           ? query.OrderByDescending(b => b.AuthorName)
                           : query.OrderBy(b => b.AuthorName);
                        break;
                    case "createddate":
                        query = desc
                        ? query.OrderByDescending(b => b.UploadDate)
                        : query.OrderBy(b => b.UploadDate);
                        break;
                    case "updateddate":
                        query = desc
                         ? query.OrderByDescending(b => b.UpdatedDate)
                         : query.OrderBy(b => b.UpdatedDate);
                        break;
                    default:
                        query = desc
                        ? query.OrderByDescending(b => b.UpdatedDate)
                        : query.OrderBy(u => u.AuthorName);
                        break;
                }
            }

            return await GetPaged(query, queryParams.PageIndex, queryParams.PageSize);

        }
    }

}
