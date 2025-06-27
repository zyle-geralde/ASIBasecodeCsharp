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
        private readonly IAuthorRepository _authorRepository;

        public LanguageService(ILanguageRepository languageRepository,IAuthorRepository authorRepository) { 
            _languageRepository = languageRepository;
            _authorRepository = authorRepository;
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

            try
            {
                Language existing_language = await _languageRepository.GetLanguageById(language.LanguageId);

                bool check_language_exist = await _languageRepository.CheckLanguageExist(language.LanguageName);

                if (check_language_exist && existing_language.LanguageName != language.LanguageName)
                {
                    throw new ArgumentException("Language Name already exist");
                }

                existing_language.LanguageName = language.LanguageName;
                existing_language.UpdatedDate = DateTime.UtcNow;

                await _languageRepository.EditLanguage();
            }
            catch(ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to edit book genre by id.");
            }
        }


        public async Task<List<BookViewModel>> GetBooksByLanguage(string languageId)
        {
            try
            {
                var all_books = await _languageRepository.GetBooksByLanguage();
                var filtered_books = all_books.Where(book => book.Language == languageId).ToList();



                var book_view_models = new List<BookViewModel>();

                foreach (var bookEntity in filtered_books)
                {
                    //Await each GetLanguageByName call sequentially
                    var languageName = (await _languageRepository.GetLanguageByName(bookEntity.Language))?.LanguageName;
                    Author authorName = await _authorRepository.GetAuthorById(bookEntity.Author != null ? bookEntity.Author : "");

                    book_view_models.Add(new BookViewModel
                    {
                        BookId = bookEntity.BookId,
                        Title = bookEntity.Title,
                        Subtitle = bookEntity.Subtitle,
                        Description = bookEntity.Description,
                        NumberOfPages = bookEntity.NumberOfPages,
                        Language = languageName, //Use the awaited languageName
                        SeriesName = bookEntity.SeriesName,
                        SeriesDescription = bookEntity.SeriesDescription,
                        SeriesOrder = bookEntity.SeriesOrder,
                        GenreList = bookEntity.GenreList,

                        // Firebase Storage URLs are directly mapped
                        CoverImageUrl = bookEntity.CoverImage,
                        BookFileUrl = bookEntity.BookFile,

                        // Parse dates from string (assuming "yyyy-MM-dd" or similar from frontend)
                        UpdatedDate = bookEntity.UpdatedDate,
                        PublicationDate = bookEntity.PublicationDate,

                        // Handle comma-separated strings
                        Publisher = bookEntity.Publisher, // Store as string
                        PublicationLocation = bookEntity.PublicationLocation, // Store as string
                        Author = authorName != null ? authorName.AuthorName : "", // Store as string
                        ISBN10 = bookEntity.ISBN10,
                        ISBN13 = bookEntity.ISBN13,
                        Edition = bookEntity.Edition,
                        CreatedBy = "admin1",
                        UpdatedBy = "Logged Admin"
                    });
                }

                return book_view_models;

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task<LanguageViewModel> GetLanguageByName(string languageId)
        {
            if (string.IsNullOrEmpty(languageId))
            {
                throw new ArgumentNullException(nameof(languageId), "Language Name should not be null");
            }

            try
            {
                Language retreived_language= await _languageRepository.GetLanguageByName(languageId);

                LanguageViewModel mapped_language = new LanguageViewModel
                {
                    LanguageId = retreived_language.LanguageId,
                    LanguageName = retreived_language.LanguageName,
                    UpdatedDate = retreived_language.UpdatedDate,
                    UpdatedBy = retreived_language.UpdatedBy,
                    CreatedBy = retreived_language.CreatedBy
                };

                return mapped_language;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve language by name.");
            }
        }
    }
}
