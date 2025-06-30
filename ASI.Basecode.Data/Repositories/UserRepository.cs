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

        public async Task<List<User>> GetUsersQueried(UserQueryParams? queryParams = null)
        {
            queryParams ??= new UserQueryParams();

            IQueryable<User> query = this.GetDbSet<User>().AsNoTracking(); 
            
            // Apply search filter if provided
            if (!string.IsNullOrEmpty(queryParams.SearchTerm))
            {
                var term = queryParams.SearchTerm.Trim();
                query = query.Where(b =>
                    (b.Id.ToString() != null && b.Id.ToString().Contains(term)) || 
                    (b.UserName != null && b.UserName.Contains(term)) || 
                    (b.Email != null && b.Email.Contains(term)));
            }

            // Apply sorting if provided
            if (!string.IsNullOrEmpty(queryParams.SortOrder))
            {
                bool desc = queryParams.SortDescending;
                switch (queryParams.SortOrder.ToLower())
                {
                    case "id":
                        query = desc
                           ? query.OrderByDescending(b => b.Id)
                           : query.OrderBy(b => b.Id);
                        break;
                    case "createdtime":
                        query = desc
                        ? query.OrderByDescending(b => b.CreatedTime)
                        : query.OrderBy(b => b.CreatedTime);
                        break;

                    case "email":
                        query = desc
                           ? query.OrderByDescending(b => b.Email)
                           : query.OrderBy(b => b.Email);
                        break;
                    default:
                        query = query.OrderBy(u => u.Id); // Default sort by ID
                        break;
                }
            }

            query = query.Skip((queryParams.PageIndex - 1) * queryParams.PageSize).Take(queryParams.PageSize);
           

            //// Skip pagination if pageSize is int.MaxValue (to get all users)
            //if (pageSize < int.MaxValue)
            //{
            //    query = query
            //        .Skip((pageIndex - 1) * pageSize)
            //        .Take(pageSize);
            //}

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

        public async Task UpdateUser(User user)
        {
            GetDbSet<User>().Update(user);
            await UnitOfWork.SaveChangesAsync();
        }
    }
}
