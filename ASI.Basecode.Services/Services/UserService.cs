using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ASI.Basecode.Resources.Constants.Enums;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Security.Cryptography;
using NetTopologySuite.Geometries;
using System.Data;
using ASI.Basecode.Data.Repositories;
using static System.Net.WebRequestMethods;
using System.Security.Policy;
using System.Data.Entity;

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPersonProfileService _personProfileService;
        private readonly IPersonProfileRepository _personProfileRepository;
        private readonly IEmailSender _emailSenderService;
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SessionManager _sessionManager;
        public UserService(IUserRepository repository, IMapper mapper, IPersonProfileService personProfileService, IPersonProfileRepository personProfileRepository, IEmailSender emailSenderService, IReviewRepository reviewRepository, IBookRepository bookRepository)
        {
            _mapper = mapper;
            _repository = repository;
            _personProfileService = personProfileService;
            _personProfileRepository = personProfileRepository;
            _emailSenderService = emailSenderService;
            _httpContextAccessor = httpContextAccessor;
            this._sessionManager = new SessionManager(httpContextAccessor.HttpContext.Session);
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
        }


        private string GenerateOtpCode()
        {
            //Generate 6-digit numeric OTP
            var otpBytes = new byte[4]; // Enough to get a number up to 2^32 - 1
            RandomNumberGenerator.Fill(otpBytes);
            var otp = BitConverter.ToUInt32(otpBytes, 0) % 1_000_000; //Get a number between 0 and 999,999
            return otp.ToString("D6"); //Format as a 6-digit string, padding with leading zeros if necessary
        }
        public async Task<string> SendOtpCodeEmail(string email)
        {
            string otp = GenerateOtpCode();
            var subject = "BasaBuzz 6 digit code";
            var message = "This is your code " + otp;

            try
            {
                await _emailSenderService.SendEmailAsync(email, subject, message);
                return otp;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> IsEmailVerifiedAndExists(string email)
        {
            var user = await _repository.FindUserByEmail(email);
            return user != null && user.IsEmailVerified;
        }

        public async Task<bool> IsUsernameVerifiedAndExists(string username)
        {
            return _repository.GetUsers().Any(u => u.UserName == username && u.IsEmailVerified);
        }

        public async Task<User> CreateVerifiedUser(UserViewModel model, string role = "User")
        {
            // Check if email or username exists among verified users
            if (await IsEmailVerifiedAndExists(model.Email))
            {
                throw new InvalidDataException("Email already exists!");
            }

            if (await IsUsernameVerifiedAndExists(model.UserName))
            {
                throw new InvalidDataException("Username already exists!");
            }

            // Delete any existing unverified account with same email or username
            var existingUnverifiedUser = await _repository.FindUserByEmail(model.Email);
            if (existingUnverifiedUser != null)
            {
                await _repository.DeleteUser(existingUnverifiedUser.Email);

                try
                {
                    await _personProfileService.DeletePersonProfile(existingUnverifiedUser.Email);
                }
                catch
                {
                    // Profile might not exist
                }
            }


            var user = new User
            {
                Email = model.Email,
                UserName = model.UserName,
                Password = PasswordManager.EncryptPassword(model.Password),
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                CreatedBy = System.Environment.UserName,
                UpdatedBy = System.Environment.UserName,
                IsEmailVerified = true,
                OtpCode = null,
                Role = role,
                OtpExpirationDate = null
            };

            await _repository.AddUser(user);

            // Create profile
            var profile = new PersonProfile
            {
                ProfileID = user.Email,
                FirstName = role == "Admin" ? model.UserName : null,
                LastName = null,
                MiddleName = null,
                Suffix = null,
                Gender = null,
                BirthDate = null,
                Location = null,
                Role = role,
                AboutMe = string.Empty
            };

            await _personProfileService.AddPersonProfile(profile);

            return user;
        }

        public LoginResult AuthenticateUser(string userId, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetUsers().Where(x => x.Email == userId &&
                                                     x.Password == passwordKey).FirstOrDefault();
            if (user == null)
            {
                return LoginResult.Failed;
            }

            return user.IsEmailVerified == true ? LoginResult.Success : LoginResult.Failed;
        }

        public async Task<PaginatedList<User>> GetUsersQueried(UserQueryParams queryParams)
        {
            return await _repository.GetUsersQueried(queryParams);
            //return users.ToList();

        }
        public async Task<User> AddUser(UserViewModel model)
        {
            var user = new User();
            if (!_repository.UserExists(model.Email))
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.Password = PasswordManager.EncryptPassword(model.Password);
                user.CreatedTime = DateTime.Now;
                user.UpdatedTime = DateTime.Now;
                user.CreatedBy = System.Environment.UserName;
                user.UpdatedBy = System.Environment.UserName;
                user.IsEmailVerified = false;
                user.OtpCode = await GenerateOtpCode(model.Email);
                user.OtpExpirationDate = DateTime.UtcNow.AddMinutes(2);

                await _repository.AddUser(user);
                return user;
            }
            else
            {
                throw new InvalidDataException(Resources.Messages.Errors.UserExists);
            }
        }

        public async Task<User> AddUserFromAdmin(UserViewModel model)
        {
            if (_repository.UserExists(model.Email))
                throw new InvalidDataException("A user with this email already exists!");

            if (_repository.UserNameExists(model.UserName))
                throw new InvalidDataException("A user with this username already exists!");

            var user = new User();
            if (!_repository.UserExists(model.Email))
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.Password = PasswordManager.EncryptPassword(model.Password);
                user.CreatedTime = DateTime.Now;
                user.UpdatedTime = DateTime.Now;
                user.CreatedBy = System.Environment.UserName;
                user.UpdatedBy = System.Environment.UserName;
                user.IsEmailVerified = true;
                user.OtpCode = null;
                user.Role = model.Role;
                user.OtpExpirationDate = null;

                await _repository.AddUser(user);

                var profile = new PersonProfile
                {
                    ProfileID = user.Email,
                    FirstName = null,
                    LastName = null,
                    MiddleName = null,
                    Suffix = null,
                    Gender = null,
                    BirthDate = null,
                    Location = null,
                    Role = model.Role,
                    AboutMe = string.Empty
                };
                await _personProfileService.AddPersonProfile(profile);

                return user;
            }
            else
            {
                throw new InvalidDataException(Resources.Messages.Errors.UserExists);
            }
        }

        public async Task<User> GetUserById(int id)
        {
            return await _repository.GetUserById(id);
        }

        public async Task<bool> UpdateUser(UserViewModel model)
        {
            try
            {
                var user = await _repository.GetUserById(model.Id);
                if (user == null)
                {
                    throw new InvalidDataException("User not found.");
                }

                if (_repository.GetUsers().Any(u => u.UserName == model.UserName && u.Id != model.Id))
                    throw new InvalidDataException("A user with this username already exists!");

                user.UserName = model.UserName;
                user.UpdatedTime = DateTime.Now;
                user.UpdatedBy = System.Environment.UserName;

                // Update password if provided
                if (!string.IsNullOrEmpty(model.Password))
                {
                    user.Password = PasswordManager.EncryptPassword(model.Password);
                }

                await _repository.UpdateUser(user);
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"{ex.Message}");
            }
        }

        public async Task<User> AddUserFromRegister(UserViewModel model)
        {
            var user = new User();
            if (!_repository.UserExists(model.Email))
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.Password = PasswordManager.EncryptPassword(model.Password);
                user.CreatedTime = DateTime.Now;
                user.UpdatedTime = DateTime.Now;
                user.CreatedBy = System.Environment.UserName;
                user.UpdatedBy = System.Environment.UserName;
                user.IsEmailVerified = false;
                user.OtpCode = await GenerateOtpCode(model.Email);
                user.OtpExpirationDate = DateTime.UtcNow.AddMinutes(2);

                await _repository.AddUser(user);


                var profile = new PersonProfile
                {
                    ProfileID = user.Email,
                    FirstName = null,        // or model.FirstName if separate
                    LastName = null,
                    MiddleName = null,
                    Suffix = null,
                    Gender = null,
                    BirthDate = null,
                    Location = null,
                    Role = "User",
                    AboutMe = string.Empty
                };
                await _personProfileService.AddPersonProfile(profile);

                return user;
            }
            else
            {
                bool verified_email = await _repository.CheckEmailVerified(model.Email);
                if (!verified_email)
                {
                    User get_user = await _repository.FindUserByEmail(model.Email);
                    string old_user_email = get_user.Email;
                    if (get_user != null)
                    {
                        get_user.Email = model.Email;
                        get_user.UserName = model.UserName;
                        get_user.Password = PasswordManager.EncryptPassword(model.Password);
                        get_user.CreatedTime = DateTime.Now;
                        get_user.UpdatedTime = DateTime.Now;
                        get_user.CreatedBy = System.Environment.UserName;
                        get_user.UpdatedBy = System.Environment.UserName;
                        get_user.IsEmailVerified = false;
                        get_user.OtpCode = await GenerateOtpCode(model.Email);
                        get_user.OtpExpirationDate = DateTime.UtcNow.AddMinutes(2);

                        await _repository.UpdateUser(get_user);

                    }
                    else
                    {
                        throw new Exception("User Not Found");
                    }

                    PersonProfile get_profile = await _personProfileService.GetPersonProfile(old_user_email);
                    if (get_profile != null)
                    {
                        get_profile.ProfileID = model.Email;
                        get_profile.FirstName = model.UserName;
                        get_profile.LastName = null;
                        get_profile.MiddleName = null;
                        get_profile.Suffix = null;
                        get_profile.Gender = null;
                        get_profile.BirthDate = null;
                        get_profile.Location = null;
                        get_profile.Role = "User";
                        get_profile.AboutMe = string.Empty;

                        await _personProfileRepository.EditPersonProfile(get_profile);
                    }
                    else
                    {
                        throw new Exception("User Not Found");
                    }


                    return get_user;

                }
                else
                {
                    throw new InvalidDataException(Resources.Messages.Errors.UserExists);
                }
            }
        }

        public async Task<User> AddAdminFromRegister(UserViewModel model)
        {
            var user = new User();
            if (!_repository.UserExists(model.Email))
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.Password = PasswordManager.EncryptPassword(model.Password);
                user.CreatedTime = DateTime.Now;
                user.UpdatedTime = DateTime.Now;
                user.CreatedBy = System.Environment.UserName;
                user.UpdatedBy = System.Environment.UserName;
                user.IsEmailVerified = false;
                user.OtpCode = await GenerateOtpCode(model.Email);
                user.OtpExpirationDate = DateTime.UtcNow.AddMinutes(5);

                await _repository.AddUser(user);


                var profile = new PersonProfile
                {
                    ProfileID = user.Email,
                    FirstName = model.UserName,        // or model.FirstName if separate
                    LastName = null,
                    MiddleName = null,
                    Suffix = null,
                    Gender = null,
                    BirthDate = null,
                    Location = null,
                    Role = "Admin",
                    AboutMe = string.Empty
                };
                await _personProfileService.AddPersonProfile(profile);

                return user;
            }
            else
            {
                bool verified_email = await _repository.CheckEmailVerified(model.Email);
                if (!verified_email)
                {
                    User get_user = await _repository.FindUserByEmail(model.Email);
                    string old_user_email = get_user.Email;
                    if (get_user != null)
                    {
                        get_user.Email = model.Email;
                        get_user.UserName = model.UserName;
                        get_user.Password = PasswordManager.EncryptPassword(model.Password);
                        get_user.CreatedTime = DateTime.Now;
                        get_user.UpdatedTime = DateTime.Now;
                        get_user.CreatedBy = System.Environment.UserName;
                        get_user.UpdatedBy = System.Environment.UserName;
                        get_user.IsEmailVerified = false;
                        get_user.OtpCode = await GenerateOtpCode(model.Email);
                        get_user.OtpExpirationDate = DateTime.UtcNow.AddMinutes(5);

                        await _repository.UpdateUser(get_user);

                    }
                    else
                    {
                        throw new Exception("User Not Found");
                    }

                    PersonProfile get_profile = await _personProfileService.GetPersonProfile(old_user_email);
                    if (get_profile != null)
                    {
                        get_profile.ProfileID = model.Email;
                        get_profile.FirstName = model.UserName;
                        get_profile.LastName = null;
                        get_profile.MiddleName = null;
                        get_profile.Suffix = null;
                        get_profile.Gender = null;
                        get_profile.BirthDate = null;
                        get_profile.Location = null;
                        get_profile.Role = "Admin";
                        get_profile.AboutMe = string.Empty;

                        await _personProfileRepository.EditPersonProfile(get_profile);
                    }
                    else
                    {
                        throw new Exception("User Not Found");
                    }


                    return get_user;

                }
                else
                {
                    throw new InvalidDataException(Resources.Messages.Errors.UserExists);
                }
            }
        }

        public async Task VerifyOtp(OtpViewModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.OtpCode))
            {
                throw new Exception("Email or otpcode is null");
            }

            try
            {
                var user = await _repository.FindUserByEmail(model.Email);

                if (user == null)
                {
                    throw new Exception("User is null");
                }

                if (user.IsEmailVerified)
                {
                    throw new Exception("Email already verified");
                }

                // Validate OTP
                if (user.OtpCode == model.OtpCode && user.OtpExpirationDate.HasValue && user.OtpExpirationDate.Value > DateTime.UtcNow)
                {
                    user.IsEmailVerified = true;
                    user.OtpCode = null;
                    user.OtpExpirationDate = null;

                    await _repository.UpdateUser(user);

                }
                else
                {
                    throw new Exception("Expired or Invalid OTP, try a different code or try resending one");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<OtpViewModel> GetUserbyEmail(string email)
        {
            try
            {
                var user = await _repository.FindUserByEmail(email);

                OtpViewModel otp_user = new OtpViewModel
                {
                    Email = user.Email,
                    OtpCode = user.OtpCode,
                };

                return otp_user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured while getting user");
            }
        }



        public IEnumerable<User> GetAllUsers()
        {
            var users = _repository
                            .GetUsers()
                            .ToList();

            var vmList = _mapper
                            .Map<List<User>>(users)
                            .OrderBy(vm => vm.Email);

            return vmList;
        }

        public async Task<bool> DeleteUser(string userId)
        {
            var user = await _repository.FindByEmailForEdit(userId);


            if (user == null)
            {
                return false;
            }
            var userReviews = await _reviewRepository.GetReviewByUser(userId);
            var impactedBookIds = userReviews
                .Select(r => r.BookId)
                .Distinct()
                .ToList();

            await _repository.DeleteUser(userId);
            if (!string.IsNullOrEmpty(user.Email))
            {
                await _personProfileService.DeletePersonProfile(user.Email);
            }
            foreach(var bookId in impactedBookIds)
            {
                await _bookRepository.calculateAverageRating(bookId);
            }
            return true;
        }
        public async Task<UserViewModel?> GetByEmailForEdit(string email)
        {
            var user = await _repository.FindByEmailForEdit(email);
            if (user == null) return null;

            return new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        private async Task<string> GenerateOtpCode(string email)
        {

            //Generate 6-digit numeric OTP
            //RNGCryptoServiceProvider for strong randomness
            var otpBytes = new byte[4]; // Enough to get a number up to 2^32 - 1
            RandomNumberGenerator.Fill(otpBytes);
            var otp = BitConverter.ToUInt32(otpBytes, 0) % 1_000_000; //Get a number between 0 and 999,999
            var otp_string = otp.ToString("D6");
            var subject = "BasaBuzz 6 digit code";
            var message = "This is your code " + otp_string;
            try
            {
                await _emailSenderService.SendEmailAsync(email, subject, message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return otp_string; //Format as a 6-digit string, padding with leading zeros if necessary
        }

        public async Task<OtpViewModel> RegenerateOtpAsync(string email)
        {
            var user = await _repository.FindUserByEmail(email);
            if (user == null)
            {
                throw new ArgumentException("User not found for OTP regeneration.");
            }
            if (user.IsEmailVerified == true)
            {
                throw new ArgumentException("Email is already verified");
            }

            user.OtpCode = await GenerateOtpCode(email);
            user.OtpExpirationDate = DateTime.UtcNow.AddMinutes(5); // New expiry
            await _repository.UpdateUser(user);

            OtpViewModel user_otp = new OtpViewModel
            {
                Email = user.Email,
                OtpCode = user.OtpCode,
            };
            return user_otp;
        }

        public async Task<string> SendOTPForResetPassword(string email)
        {
            var user = await _repository.FindUserByEmail(email);

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email is empty for OTP regeneration.");
            }
            if(user == null)
            {
                throw new ArgumentException("Email not found for OTP regeneration.");
            }
            if (user.IsEmailVerified == false)
            {
                throw new ArgumentException("Email is not verified");
            }

            try
            {
                string generatedOtp = await GenerateOtpCode(email);
                return generatedOtp;
            }catch(Exception ex)
            {
                throw;
            }


        }

        public async Task UpdatePassword(UserViewModel user)
        {
            if(user == null)
            {
                throw new ArgumentException("User is empty");
            }

            var foundUser = await _repository.FindUserByEmail(user.Email);

            if (foundUser == null) {
                throw new ArgumentException("User could not be found");
            }

            try
            {

                foundUser.Password = PasswordManager.EncryptPassword(user.Password);
                

                await _repository.UpdatePassword(foundUser);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public async Task<bool> ChangePassword(int id, string currentPassword, string newPassword)
        {
            var user = await _repository.GetUserById(id);
            if (user == null) return false;

            // returns error if the current password doesnt match
            var currentHash = PasswordManager.EncryptPassword(currentPassword);
            if (user.Password != currentHash)
                return false;

            user.Password = PasswordManager.EncryptPassword(newPassword);
            await _repository.UpdateUser(user);
            return true;
        }
    }
}
