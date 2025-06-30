using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        LoginResult AuthenticateUser(string userid, string password, ref User user);
        //Task<User> AddUser(UserViewModel model);
        Task<User> AddUser(UserViewModel model);
        Task<User> AddUserFromAdmin(UserViewModel model);
        Task<User> GetUserById(int id);
        Task<bool> UpdateUser(UserViewModel model);
        Task<User> AddUserFromRegister(UserViewModel model);
        Task<User> AddAdminFromRegister(UserViewModel model);
        IEnumerable<User> GetAllUsers();
        Task<bool> DeleteUser(int id);
        Task<List<User>> GetUsersQueried(string searchTerm, string sortOrder, int pageIndex, int pageSize);
        Task VerifyOtp(OtpViewModel model);

        Task<OtpViewModel> GetUserbyEmail(string email);
        Task<OtpViewModel> RegenerateOtpAsync(string email);

    }
}
