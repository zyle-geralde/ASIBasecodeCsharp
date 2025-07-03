using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.QueryParams;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface ILanguageService
    {
        Task AddLanguage(LanguageViewModel language);

        Task<List<LanguageViewModel>> GetAllLanguage();

        Task DeleteLanguage(string languageId);

        Task EditLanguage(LanguageViewModel language);


        Task<List<BookViewModel>> GetBooksByLanguage(string languageId);

        Task<LanguageViewModel> GetLanguageByName(string languageId);

        Task<PaginatedList<Language>> GetLanguageQueried(LanguageQueryParams queryParams);
    }
}
