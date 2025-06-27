using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Data.Entity;

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

        public async Task<List<User>> GetUsersQueried(string searchTerm, string sortOrder, int pageIndex, int pageSize)
        {
            IQueryable<User> query = this.GetDbSet<User>().AsNoTracking();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(b =>
                    (b.Id.ToString() != null && b.Id.ToString().Contains(term))|| (b.UserName != null && b.UserName.Contains(term)) || (b.Email != null && b.Email.Contains(term)));
            }

            query = query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            return await query.ToListAsync();
        }

        public bool UserExists(string userId)
        {
            return this.GetDbSet<User>().Any(x => x.Email == userId);
        }
        public async Task<bool> CheckEmailVerified(string email)
        {
            User existing_user = await FindUserByEmail(email);

            return existing_user.IsEmailVerified;
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

        public bool UserNameExists(string userName)
        {
            return this.GetDbSet<User>().Any(x => x.UserName == userName);
        }

        public async Task<bool> IsUsernameAvailable(string username, int? currentUserId = null)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            // Use the GetUsers method directly from this class, not from _repository
            var users = GetUsers();

            // Find user with matching username
            var existingUser = await users.FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());

            // If no user found with this username, it's available
            if (existingUser == null)
                return true;

            // If we're editing a user and the username belongs to that same user, it's available
            if (currentUserId.HasValue && existingUser.Id == currentUserId.Value)
                return true;

            // Username exists and belongs to another user
            return false;
        }

        public async Task UpdateUser(User user)
        {
            GetDbSet<User>().Update(user);
            await UnitOfWork.SaveChangesAsync();
        }
    }
}
