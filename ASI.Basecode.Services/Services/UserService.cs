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

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPersonProfileService _personProfileService;
        private readonly IPersonProfileRepository _personProfileRepository;
        private readonly IEmailSender _emailSenderService;

        public UserService(IUserRepository repository, IMapper mapper, IPersonProfileService personProfileService, IPersonProfileRepository personProfileRepository, IEmailSender emailSenderService)
        {
            _mapper = mapper;
            _repository = repository;
            _personProfileService = personProfileService;
            _personProfileRepository = personProfileRepository;
            _emailSenderService = emailSenderService; 
        }

        public LoginResult AuthenticateUser(string userId, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetUsers().Where(x => x.Email == userId &&
                                                     x.Password == passwordKey).FirstOrDefault();
            if(user == null)
            {
                return LoginResult.Failed; 
            }

            return user.IsEmailVerified == true ? LoginResult.Success : LoginResult.Failed;
        }

        public async Task<List<User>> GetUsersQueried(string searchTerm, string sortOrder, int pageIndex, int pageSize)
        {
            var users = await _repository.GetUsersQueried(searchTerm, sortOrder, pageIndex, pageSize);
            return users.ToList();
            
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
                    FirstName = model.UserName,        // or model.FirstName if separate
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
                    if (get_profile != null) {
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
                user.OtpCode =  await GenerateOtpCode(model.Email);
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

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _repository.GetUserById(id);

            if (user == null)
            {
                return false;
            }

            await _repository.DeleteUser(id);
            return true;
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
            catch (Exception ex) {
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
            if(user.IsEmailVerified == true)
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
    }
}
