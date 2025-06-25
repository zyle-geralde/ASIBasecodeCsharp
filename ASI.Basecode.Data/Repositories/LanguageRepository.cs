using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ASI.Basecode.Data.Repositories
{
    public class LanguageRepository:ILanguageRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;

        public LanguageRepository(AsiBasecodeDBContext dbContext)
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
    }
}
