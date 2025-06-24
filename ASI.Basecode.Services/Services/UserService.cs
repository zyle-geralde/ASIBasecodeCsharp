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

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public LoginResult AuthenticateUser(string userId, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetUsers().Where(x => x.Email == userId &&
                                                     x.Password == passwordKey).FirstOrDefault();

            return user != null ? LoginResult.Success : LoginResult.Failed;
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

                await _repository.AddUser(user);
                return user;
            }
            else
            {
                throw new InvalidDataException(Resources.Messages.Errors.UserExists);
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
    }
}
