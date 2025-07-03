using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using ASI.Basecode.Data.QueryParams;
using Basecode.Data.Repositories;

namespace ASI.Basecode.Data.Repositories
{
    public class LanguageRepository: BaseRepository,ILanguageRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;

        public LanguageRepository(AsiBasecodeDBContext dbContext,IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        public async Task AddLanguage(Language language)
        {
            await _dbContext.Languages.AddAsync(language);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> CheckLanguageExist(string language)
        {
            return await _dbContext.Languages.AnyAsync(language_param => language_param.LanguageName.ToLower() == language.ToLower());
        }

        public async Task<List<Language>> GetAllLanguage()
        {
            try
            {
                return await _dbContext.Languages.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Language> GetLanguageById(string languageId)
        {
            return await _dbContext.Languages.FirstOrDefaultAsync(language => language.LanguageId == languageId);
        }

        public async Task DeleteLanguage(string languageId)
        {
            Language existingLanguage = await GetLanguageById(languageId);

            _dbContext.Languages.Remove(existingLanguage);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EditLanguage()
        {
            await _dbContext.SaveChangesAsync();
        }
        

        public async Task<List<Book>> GetBooksByLanguage()
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

        public async Task<Language> GetLanguageByName(string languageId)
        {
            try
            {
                return await _dbContext.Languages.FirstOrDefaultAsync(language => language.LanguageId == languageId);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<PaginatedList<Language>> GetLanguageQueried(LanguageQueryParams? queryParams = null)
        {

            queryParams ??= new LanguageQueryParams();

            IQueryable<Language> query = this.GetDbSet<Language>().AsNoTracking();

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(queryParams.SearchTerm))
            {
                var term = queryParams.SearchTerm.Trim();
                query = query.Where(b =>
                    (b.LanguageName != null && b.LanguageName.Contains(term)));
            }

            if (!string.IsNullOrEmpty(queryParams.SortOrder))
            {
                bool desc = queryParams.SortDescending;
                switch (queryParams.SortOrder.ToLower())
                {
                    case "name":
                        query = desc
                           ? query.OrderByDescending(b => b.LanguageName)
                           : query.OrderBy(b => b.LanguageName);
                        break;
                    case "createdtime":
                        query = desc
                        ? query.OrderByDescending(b => b.UploadDate)
                        : query.OrderBy(b => b.UploadDate);
                        break;
                    default:
                        query = desc
                        ? query.OrderByDescending(b => b.LanguageName)
                        : query.OrderBy(u => u.LanguageName);
                        break;
                }
            }

            return await GetPaged(query, queryParams.PageIndex, queryParams.PageSize);

        }

    }
}
