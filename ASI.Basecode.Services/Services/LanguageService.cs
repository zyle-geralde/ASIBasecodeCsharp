using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class LanguageService:ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;

        public LanguageService(ILanguageRepository languageRepository) { 
            _languageRepository = languageRepository;
        }

        public async Task AddLanguage(LanguageViewModel language)
        {
            if (language == null) {
                throw new ArgumentException("Langugage should not be null");
            }
            if (string.IsNullOrEmpty(language.LanguageName))
            {
                throw new ArgumentException("Langugage Name should not be null");
            }
            try
            {
                bool check_language_exist = await _languageRepository.CheckLanguageExist(language.LanguageName);


                if (check_language_exist)
                {
                    throw new ArgumentException("Genre Name already exist");
                }


                var mapped_language = new Language
                {
                    LanguageId = Guid.NewGuid().ToString(),
                    LanguageName = language.LanguageName,
                    CreatedBy = "admin1",
                    UpdatedBy = "admin1",
                    UpdatedDate = DateTime.Now,
                    UploadDate = DateTime.Now,
                };

                try
                {
                    await _languageRepository.AddLanguage(mapped_language);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed in adding language to repository");
                }
            }
            catch (ArgumentException ex)
            {
                throw new ApplicationException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed in adding language");
            }

        }
    }
}
