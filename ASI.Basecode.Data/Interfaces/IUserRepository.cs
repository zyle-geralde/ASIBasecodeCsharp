using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsersQueried(string searchTerm, string sortOrder, int pageIndex, int pageSize);
        IQueryable<User> GetUsers();
        bool UserExists(string userId);
        Task AddUser(User user);
        Task<User> GetUserById(int Id);
        Task DeleteUser(int Id);

    }
}
