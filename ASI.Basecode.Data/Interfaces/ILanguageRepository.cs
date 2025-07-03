using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.QueryParams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ILanguageRepository
    {
        Task AddLanguage(Language language);

        Task<bool> CheckLanguageExist(string language);

        Task <List<Language>>GetAllLanguage();

        Task DeleteLanguage(string languageId);

        Task<Language> GetLanguageById(string languageId);

        Task EditLanguage();

        Task<List<Book>> GetBooksByLanguage();

        Task<Language> GetLanguageByName(string languageId);
        Task<PaginatedList<Language>> GetLanguageQueried(LanguageQueryParams? queryParams = null);
    }
}
