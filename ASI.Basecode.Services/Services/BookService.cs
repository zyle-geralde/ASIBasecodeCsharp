﻿using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.QueryParams;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SessionManager _sessionManager;

        public BookService(IBookRepository bookRepository, ILanguageRepository languageRepository, IAuthorRepository authorRepository, IHttpContextAccessor httpContextAccessor)
        {
            _bookRepository = bookRepository;
            _languageRepository = languageRepository;
            _authorRepository = authorRepository;
            _httpContextAccessor = httpContextAccessor;
            this._sessionManager = new SessionManager(httpContextAccessor.HttpContext.Session);
        }

        public async Task AddBook(BookViewModel request)
        {
            // Here you can add any business logic or validation before saving the book to the database.
            //example:if (string.IsNullOrWhiteSpace(book.Title)) throw new ArgumentException("Book title cannot be empty.");

            
            try
            {
                bool isBookNameAndAuthorExist = await _bookRepository.CheckBookNameAndAuthorExist(request.Author.Trim(),request.Title.Trim().ToLower());

                if (isBookNameAndAuthorExist)
                {
                    throw new ApplicationException("Book Title and/or Author Exist.");
                }
            }
            catch(ApplicationException ex)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new ApplicationException($"Failed to add book: {ex.Message}", ex);
            }

            // Map DTO to actual Book model
            //Change to mapper

            var book = new Book
            {
                BookId = Guid.NewGuid().ToString(),
                Title = request.Title,
                Subtitle = request.Subtitle,
                Description = request.Description,
                NumberOfPages = request.NumberOfPages,
                Language = request.Language,
                SeriesName = request.SeriesName,
                SeriesDescription = request.SeriesDescription,
                SeriesOrder = request.SeriesOrder,
                AverageRating = 0,
                Likes = 0,
                GenreList = request.GenreList,
                IsFeatured = request.IsFeatured,

                // Firebase Storage URLs are directly mapped
                CoverImage = request.CoverImageUrl,
                BookFile = request.BookFileUrl,

                // Parse dates from string (assuming "yyyy-MM-dd" or similar from frontend)
                UploadDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                PublicationDate = request.PublicationDate,

                // Handle comma-separated strings
                Publisher = request.Publisher, // Store as string
                PublicationLocation = request.PublicationLocation, // Store as string
                Author = request.Author, // Store as string
                ISBN10 = request.ISBN10,
                ISBN13 = request.ISBN13,
                Edition = request.Edition,
                CreatedBy = _httpContextAccessor.HttpContext.Session.GetString("UserName"),
                UpdatedBy = _httpContextAccessor.HttpContext.Session.GetString("UserName")
            };

            try
            {
                await _bookRepository.AddBook(book);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to add book: {ex.Message}", ex);
            }
        }

        public async Task<PaginatedList<BookViewModel>> GetBooks(BookQueryParams queryParams)
        {
            var books = await _bookRepository.GetBooks(queryParams);
            var bookList = new List<BookViewModel>();

            foreach (var b in books)
            {
                // Get language name if available
                Language languageName = null;
                if (!string.IsNullOrEmpty(b.Language))
                {
                    languageName = await _languageRepository.GetLanguageByName(b.Language);
                }

                // Get author name if available
                Author authorName = null;
                if (!string.IsNullOrEmpty(b.Author))
                {
                    authorName = await _authorRepository.GetAuthorById(b.Author);
                }

                bookList.Add(new BookViewModel
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Subtitle = b.Subtitle,
                    UploadDate = b.UploadDate,
                    GenreList = b.GenreList,
                    PublicationDate = b.PublicationDate,
                    Author = authorName != null ? authorName.AuthorName : "",
                    AverageRating = b.AverageRating,
                    CoverImage = b.CoverImage,
                    BookFile = b.BookFile,
                    Description = b.Description,
                    IsFeatured = b.IsFeatured,
                    Language = languageName != null ? languageName.LanguageName : "",
                    NumberOfPages = b.NumberOfPages,
                    Publisher = b.Publisher,
                    PublicationLocation = b.PublicationLocation,
                    SeriesName = b.SeriesName,
                    ReviewCount = b.ReviewCount
                });
            }

            return new PaginatedList<BookViewModel>(
               bookList,
               books.TotalCount,
               books.PageIndex,
               queryParams.PageSize
            );
        }

        public async Task<List<BookViewModel>> GetAllBooks()
        {
            List<Book> book_list = await _bookRepository.GetAllBooks();

            if (book_list == null || !book_list.Any())
            {
                return new List<BookViewModel>();
            }

            List<BookViewModel> bookViewModel_list = new List<BookViewModel>();

            foreach (var book in book_list)
            {
                Language languageName = await _languageRepository.GetLanguageByName(book.Language!=null?book.Language:"");
                Author authorName = await _authorRepository.GetAuthorById(book.Author != null ? book.Author : "");
                var viewModel = new BookViewModel
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Subtitle = book.Subtitle,
                    Description = book.Description,
                    NumberOfPages = book.NumberOfPages,
                    Language = languageName!=null?languageName.LanguageName:"",
                    SeriesName = book.SeriesName,
                    SeriesDescription = book.SeriesDescription,
                    SeriesOrder = book.SeriesOrder,
                    GenreList = book.GenreList,
                    AverageRating = book.AverageRating,
                    IsFeatured = book.IsFeatured,

                    // Firebase Storage URLs are directly mapped
                    CoverImageUrl = book.CoverImage,
                    BookFileUrl = book.BookFile,

                    // Parse dates from string (assuming "yyyy-MM-dd" or similar from frontend)
                    UpdatedDate = book.UpdatedDate,
                    PublicationDate = book.PublicationDate,


                    // Handle comma-separated strings
                    Publisher = book.Publisher, // Store as string
                    PublicationLocation = book.PublicationLocation, // Store as string
                    Author = authorName != null ? authorName.AuthorName : "", // Store as string
                    ISBN10 = book.ISBN10,
                    ISBN13 = book.ISBN13,
                    Edition = book.Edition,
                    CreatedBy = book.CreatedBy,
                    UpdatedBy = book.UpdatedBy,

                };
                bookViewModel_list.Add(viewModel);
            }

            return bookViewModel_list;
        }
    


        public async Task<BookViewModel?> GetBookById(string bookId)
        {
            Book requestBook= await _bookRepository.GetBookById(bookId);
            var book = new BookViewModel
            {
                BookId = requestBook.BookId,
                Title = requestBook.Title,
                Subtitle = requestBook.Subtitle,
                Description = requestBook.Description,
                NumberOfPages = requestBook.NumberOfPages,
                Language = requestBook.Language,
                SeriesName = requestBook.SeriesName,
                SeriesDescription = requestBook.SeriesDescription,
                SeriesOrder = requestBook.SeriesOrder,
                GenreList= requestBook.GenreList,
                AverageRating =requestBook.AverageRating,
                IsFeatured = requestBook.IsFeatured,
                ReviewCount = requestBook.ReviewCount,

                // Firebase Storage URLs are directly mapped
                CoverImageUrl = requestBook.CoverImage,
                BookFileUrl = requestBook.BookFile,

                // Parse dates from string (assuming "yyyy-MM-dd" or similar from frontend)
                UpdatedDate = requestBook.UpdatedDate,
                PublicationDate = requestBook.PublicationDate,
                UploadDate = requestBook.UploadDate,


                // Handle comma-separated strings
                Publisher = requestBook.Publisher, // Store as string
                PublicationLocation = requestBook.PublicationLocation, // Store as string
                Author = requestBook.Author, // Store as string
                ISBN10 = requestBook.ISBN10,
                ISBN13 = requestBook.ISBN13,
                Edition = requestBook.Edition,
                CreatedBy = requestBook.CreatedBy,
                UpdatedBy = requestBook.UpdatedBy,
            };
            //return await _bookRepository.GetBookById(bookId);
            return book;
        }

        public async Task EditBook(BookViewModel request)
        {

            //Change to mapper
            var book = new Book
            {
                BookId = request.BookId,
                Title = request.Title,
                Subtitle = request.Subtitle,
                Description = request.Description,
                NumberOfPages = request.NumberOfPages,
                Language = request.Language,
                SeriesName = request.SeriesName,
                SeriesDescription = request.SeriesDescription,
                SeriesOrder = request.SeriesOrder,
                GenreList = request.GenreList,
                IsFeatured = request.IsFeatured,

                // Firebase Storage URLs are directly mapped
                CoverImage = request.CoverImageUrl,
                BookFile = request.BookFileUrl,

                // Parse dates from string (assuming "yyyy-MM-dd" or similar from frontend)
                UpdatedDate = DateTime.Now,
                PublicationDate = request.PublicationDate,

                // Handle comma-separated strings
                Publisher = request.Publisher, // Store as string
                PublicationLocation = request.PublicationLocation, // Store as string
                Author = request.Author, // Store as string
                ISBN10 = request.ISBN10,
                ISBN13 = request.ISBN13,
                Edition = request.Edition,
                UpdatedBy = _httpContextAccessor.HttpContext.Session.GetString("UserName"),
            };


            try
            {
                await _bookRepository.EditBook(book);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task DeletBook(string bookId)
        {

            try
            {
                await _bookRepository.DeleteBook(bookId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to Delete book: {ex.Message}", ex);
            }
            
        }

        public async Task<List<string>> GetAllGenres()
        {
            try
            {
                List<BookGenre> book_genre_list = await _bookRepository.GetAllGenres();


                if (book_genre_list == null || !book_genre_list.Any())
                {
                    return new List<string>();
                }

                //Mapping
                List<string> view_model_list = book_genre_list.Select(book_genre_element => book_genre_element.GenreName+','+ book_genre_element.BookGenreId).ToList();

                return view_model_list;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve book genres.", ex);
            }
        }

        public async Task<List<string>> GetAllLanguage()
        {
            try
            {
                List<Language> language_list = await _bookRepository.GetAllLanguage();


                if (language_list == null || !language_list.Any())
                {
                    return new List<string>();
                }

                //Mapping
                List<string> view_model_list = language_list.Select(book_language_element => book_language_element.LanguageName + ',' + book_language_element.LanguageId).ToList();

                return view_model_list;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve language genres.", ex);
            }
        }

        public async Task<List<string>> GetAllAuthor()
        {
            try
            {
                List<Author> author_list = await _authorRepository.GetAllAuthorList();


                if (author_list == null || !author_list.Any())
                {
                    return new List<string>();
                }

                //Mapping
                List<string> view_model_list = author_list.Select(author_list_element => author_list_element.AuthorName + ',' + author_list_element.AuthorId).ToList();

                return view_model_list;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to retrieve authors.", ex);
            }
        }

    }
}
