using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IAuthorService
    {
        Task AddAuthor(AuthorViewModel author);
        Task<List<AuthorViewModel>> GetAllAuthorList();

        Task EditAuthor(AuthorViewModel author);

        Task<AuthorViewModel> GetAuthorById(string author_id);

        Task DeleteAuthor(string author_id);

        Task<List<BookViewModel>> GetBooksByAuthor(string author_id);

    }
}
