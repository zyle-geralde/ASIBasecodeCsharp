using ASI.Basecode.Data;
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
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPersonProfileRepository _personProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IPersonProfileRepository personProfileRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _personProfileRepository = personProfileRepository;
            _unitOfWork = unitOfWork;
        }

        public LoginResult AuthenticateUser(string userId, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _userRepository.GetUsers().Where(x => x.UserId == userId &&
                                                     x.Password == passwordKey).FirstOrDefault();

            return user != null ? LoginResult.Success : LoginResult.Failed;
        }

        public async Task AddUser(UserViewModel model)
        {
            var user = new User();

            _mapper.Map(model, user);
            user.Password = PasswordManager.EncryptPassword(model.Password);
            user.CreatedTime = DateTime.Now;
            user.UpdatedTime = DateTime.Now;
            user.CreatedBy = System.Environment.UserName;
            user.UpdatedBy = System.Environment.UserName;

            await _userRepository.AddUser(user);

            var profile = new PersonProfile
            {
                ProfileID = user.UserId
            };
            await _personProfileRepository.AddProfile(profile);

            await _unitOfWork.SaveChangesAsync();

        }


        public IEnumerable<User> GetAllUsers()
        {
            var users = _userRepository
                            .GetUsers()
                            .ToList();

            var vmList = _mapper
                            .Map<List<User>>(users)
                            .OrderBy(vm => vm.UserId);

            return vmList;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _userRepository.GetUserById(id);

            if (user == null)
            {
                return false;
            }

            await _userRepository.DeleteUser(id);
            return true;
        }
    }
}
