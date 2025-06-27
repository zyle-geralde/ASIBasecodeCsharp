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
    public class AuthorService:IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
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
                    CreatedBy = "admin1",
                    UpdatedBy = "admin1",
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
    }
}
