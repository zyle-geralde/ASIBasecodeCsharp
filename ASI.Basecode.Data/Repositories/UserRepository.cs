using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ASI.Basecode.Data.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork) 
        {

        }

        public IQueryable<User> GetUsers()
        {
            return this.GetDbSet<User>();
        }

        public bool UserExists(string userId)
        {
            return this.GetDbSet<User>().Any(x => x.Email == userId);
        }

        public async Task AddUser(User user)
        {
            GetDbSet<User>().Add(user);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await this.GetDbSet<User>().FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task DeleteUser(int id)
        {
            var user = await GetUserById(id);
            if (user == null) return;

            this.GetDbSet<User>().Remove(user);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<User> FindUserByEmail(string email)
        {
            return await this.GetDbSet<User>().FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateUser(User user)
        {
            GetDbSet<User>().Update(user);
            await UnitOfWork.SaveChangesAsync();
        }
    }
}
