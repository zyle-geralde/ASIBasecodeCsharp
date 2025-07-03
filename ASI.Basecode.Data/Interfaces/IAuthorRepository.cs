using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.QueryParams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IAuthorRepository
    {
        Task AddAuthor(Author author);

        Task<bool> CheckAuthorExist(string author_name);

        Task<List<Author>> GetAllAuthorList();

        Task EditAuthor();

        Task<Author> GetAuthorById(string author_id);

        Task DeleteAuthor(string auhtor_id);


        Task<List<Book>> GetBooksByAuthor();

        Task<string> GetAuthorByName(string author_name);
        Task<PaginatedList<Author>> GetAuthorQueried(AuthorQueryParams? queryParams = null);


    }
}
