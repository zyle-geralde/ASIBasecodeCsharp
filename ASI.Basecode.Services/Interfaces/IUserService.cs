using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.QueryParams;
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
        Task<PaginatedList<User>> GetUsersQueried(UserQueryParams queryParams);
        Task VerifyOtp(OtpViewModel model);

        Task<OtpViewModel> GetUserbyEmail(string email);
        Task<OtpViewModel> RegenerateOtpAsync(string email);
        Task<UserViewModel?> GetByEmailForEdit(string email);
        Task<bool> ChangePassword(int userId, string currentPassword, string newPassword);

        Task<string> SendOTPForResetPassword(string email);

        Task UpdatePassword(UserViewModel user);

        Task<bool> IsEmailVerifiedAndExists(string email);
        Task<bool> IsUsernameVerifiedAndExists(string username);
        Task<string> SendOtpCodeEmail(string email);
        Task<User> CreateVerifiedUser(UserViewModel model, string role = "User");



    }
}
