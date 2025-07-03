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
        //Task<List<User>> GetUsersQueried(UserQueryParams queryParams);
        Task<PaginatedList<User>> GetUsersQueried(UserQueryParams? queryParams = null);
        IQueryable<User> GetUsers();
        bool UserExists(string userId);
        Task AddUser(User user);
        Task<User> GetUserById(int Id);
        Task DeleteUser(int Id);
        Task<User> FindUserByEmail(string email);
        Task UpdateUser(User user);
        bool UserNameExists(string userName);
        Task<bool> CheckEmailVerified(string email);
        Task<User> FindByEmailForEdit(string email);
        Task<bool> ChangePassword(int userId, string currentPasswordHash, string newPasswordHash);


    }
}
