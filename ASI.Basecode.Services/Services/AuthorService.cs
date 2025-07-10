using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.QueryParams;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class AuthorService:IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SessionManager _sessionManager;
        public AuthorService(IAuthorRepository authorRepository,ILanguageRepository languageRepository, IHttpContextAccessor httpContextAccessor)
        {
            _authorRepository = authorRepository;
            _languageRepository = languageRepository;
            _httpContextAccessor = httpContextAccessor;
            this._sessionManager = new SessionManager(httpContextAccessor.HttpContext.Session);
        }

        public async Task AddAuthor(AuthorViewModel author)
        {
            if(author == null)
            {
                throw new ArgumentNullException(nameof(author), "author should not be null");
            }
            if(author.AuthorName == null || author.AuthorName.Trim() == "")
            {
                throw new ArgumentException("Authorname cannot be null or empty");
            }

            bool check_author_exist = await _authorRepository.CheckAuthorExist(author.AuthorName);

            if (check_author_exist)
            {
                throw new ArgumentException("AuthorName already exist");
            }


            try
            {
                var mapped_Author = new Author
                {
                    AuthorId = Guid.NewGuid().ToString(),
                    AuthorName = author.AuthorName,
                    AuthorDescription = author.AuthorDescription,
                    AuthorImageUrl = author.AuthorImageUrl,
                    CreatedBy = _httpContextAccessor.HttpContext.Session.GetString("UserName"),
                    UpdatedBy = _httpContextAccessor.HttpContext.Session.GetString("UserName"),
                    UpdatedDate = DateTime.Now,
                    UploadDate = DateTime.Now,
                };
                await _authorRepository.AddAuthor(mapped_Author);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to add author", ex);
            }
        }

        public async Task<PaginatedList<Author>> GetAuthorQueried(AuthorQueryParams queryParams)
        {
            return await _authorRepository.GetAuthorQueried(queryParams);
            //return users.ToList();

        }


        public async Task<List<AuthorViewModel>> GetAllAuthorList()
        {
            List<Author> author_list = await _authorRepository.GetAllAuthorList();


            if (author_list == null || !author_list.Any())
            {
                return new List<AuthorViewModel>();
            }

            //Mapping
            List<AuthorViewModel> view_model_list = author_list.Select(author_element => new AuthorViewModel
            {
                AuthorId = author_element.AuthorId,
                AuthorName = author_element.AuthorName,
                AuthorDescription = author_element.AuthorDescription,
                AuthorImageUrl = author_element.AuthorImageUrl,
                CreatedBy = author_element.CreatedBy,
                UpdatedDate = author_element.UpdatedDate,
                UploadDate = author_element.UploadDate,
                UpdatedBy = author_element.UpdatedBy
            }).ToList();


            return view_model_list;
        }

        public async Task EditAuthor(AuthorViewModel author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author), "Author should not be null");
            }


            try
            {
                Author existing_author = await _authorRepository.GetAuthorById(author.AuthorId);

                bool check_author_exist = await _authorRepository.CheckAuthorExist(author.AuthorName);

                if (check_author_exist && existing_author.AuthorName != author.AuthorName)
                {
                    throw new ArgumentException("Author Name already exist");
                }

                existing_author.AuthorName = author.AuthorName;
                existing_author.AuthorDescription = author.AuthorDescription;
                existing_author.AuthorImageUrl = author.AuthorImageUrl;
                existing_author.UpdatedDate = DateTime.Now;
                existing_author.UpdatedBy = _httpContextAccessor.HttpContext.Session.GetString("UserName");

                await _authorRepository.EditAuthor();
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to edit Author by id.", ex);
            }
        }



        public async Task<AuthorViewModel> GetAuthorById(string author_id)
        {

            if (string.IsNullOrEmpty(author_id))
            {
                throw new ArgumentNullException(nameof(author_id), "Author Id should not be null");
            }

            try
            {
                Author retreived_author = await _authorRepository.GetAuthorById(author_id);
                if (retreived_author == null) {
                    AuthorViewModel mapped_author = new AuthorViewModel
                    {
                        AuthorId = "",
                        AuthorName = "",
                        AuthorDescription = "",
                        AuthorImageUrl = "",
                    };
                    return mapped_author;

                }
                else
                {
                    AuthorViewModel mapped_author = new AuthorViewModel
                    {
                        AuthorId = retreived_author.AuthorId,
                        AuthorName = retreived_author.AuthorName,
                        AuthorDescription = retreived_author.AuthorDescription,
                        AuthorImageUrl = retreived_author.AuthorImageUrl,
                    };

                    return mapped_author;
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve book author by id.", ex);
            }
        }

        public async Task DeleteAuthor(string author_id)
        {
            try
            {
                await _authorRepository.DeleteAuthor(author_id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to delete author: {ex.Message}", ex);
            }
        }
        public async Task<List<BookViewModel>> GetBooksByAuthor(string author_id)
        {
            try
            {
                var all_books = await _authorRepository.GetBooksByAuthor();
                var filtered_books = all_books.Where(book => book.Author == author_id).ToList();

                var book_view_models = new List<BookViewModel>();

                foreach (var bookEntity in filtered_books)
                {
                    //Await each GetLanguageByName call sequentially
                    var languageName = await _languageRepository.GetLanguageByName(bookEntity != null ? bookEntity.Language : "");
                    Author authorName = await _authorRepository.GetAuthorById(bookEntity.Author != null ? bookEntity.Author : "");

                    book_view_models.Add(new BookViewModel
                    {
                        BookId = bookEntity.BookId,
                        Title = bookEntity.Title,
                        Subtitle = bookEntity.Subtitle,
                        Description = bookEntity.Description,
                        NumberOfPages = bookEntity.NumberOfPages,
                        Language = languageName != null ? languageName.LanguageName : "", //Use the awaited languageName
                        SeriesName = bookEntity.SeriesName,
                        SeriesDescription = bookEntity.SeriesDescription,
                        SeriesOrder = bookEntity.SeriesOrder,
                        GenreList = bookEntity.GenreList,
                        IsFeatured = bookEntity.IsFeatured,

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
                        CreatedBy = bookEntity.CreatedBy,
                        UpdatedBy = bookEntity.UpdatedBy
                    });
                }

                return book_view_models;

            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to Get books by genre: {ex.Message}", ex);
            }
        }

    }
}
