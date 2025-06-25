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
                    throw new ArgumentException("Language Name already exist");
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


        public async Task<List<LanguageViewModel>> GetAllLanguage()
        {

            List<Language> language_list = await _languageRepository.GetAllLanguage();


            if (language_list == null || !language_list.Any())
            {
                return new List<LanguageViewModel>();
            }

            //Mapping
            List<LanguageViewModel> view_model_list = language_list.Select(language_element => new LanguageViewModel
            {
                LanguageId = language_element.LanguageId,
                LanguageName = language_element.LanguageName,
                CreatedBy = language_element.CreatedBy,
                UpdatedDate = language_element.UpdatedDate,
                UploadDate = language_element.UploadDate,
                UpdatedBy = language_element.UpdatedBy
            }).ToList();


            return view_model_list;
        }

        public async Task DeleteLanguage(string languageId)
        {

            try
            {
                await _languageRepository.DeleteLanguage(languageId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to Delete Language");
            }

        }


        public async Task EditLanguage(LanguageViewModel language)
        {
            if (language == null)
            {
                throw new ArgumentNullException(nameof(language), "Language should not be null");
            }
            bool check_language_exist = await _languageRepository.CheckLanguageExist(language.LanguageName);

            if (check_language_exist)
            {
                throw new ArgumentException("Language Name already exist");
            }
            try
            {
                Language existing_language = await _languageRepository.GetLanguageById(language.LanguageId);

                existing_language.LanguageName = language.LanguageName;
                existing_language.UpdatedDate = DateTime.UtcNow;

                await _languageRepository.EditLanguage();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to edit book genre by id.");
            }
        }
    }
}
