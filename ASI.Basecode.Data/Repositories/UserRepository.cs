using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<User> FindByEmailForEdit(string email)
        {
            return await this.GetDbSet<User>()
                                   .SingleOrDefaultAsync(u => u.Email == email);
        }
        public async Task<PaginatedList<User>> GetUsersQueried(UserQueryParams? queryParams = null)
        {
            queryParams ??= new UserQueryParams();

            IQueryable<User> query = this.GetDbSet<User>().AsNoTracking(); 
            
            if (!string.IsNullOrEmpty(queryParams.SearchTerm))
            {
                var term = queryParams.SearchTerm.Trim();
                query = query.Where(b =>
                    (b.Id.ToString() != null && b.Id.ToString().Contains(term)) || 
                    (b.UserName != null && b.UserName.Contains(term)) || 
                    (b.Email != null && b.Email.Contains(term)));
            }
            if (!string.IsNullOrEmpty(queryParams.Role))
            {
                query = query.Where(u => u.Role == queryParams.Role);
            }
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
                    case "createddate":
                        query = desc
                        ? query.OrderByDescending(b => b.CreatedTime)
                        : query.OrderBy(b => b.CreatedTime);
                        break;
                    case "updateddate":
                        query = desc
                        ? query.OrderByDescending(b => b.UpdatedTime)
                        : query.OrderBy(b => b.UpdatedTime);
                        break;
                    case "email":
                        query = desc
                           ? query.OrderByDescending(b => b.Email)
                           : query.OrderBy(b => b.Email);
                        break;
                    default:
                        query = query.OrderBy(u => u.CreatedTime);
                        break;
                }
            }

            return await GetPaged(query, queryParams.PageIndex, queryParams.PageSize);
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

        public async Task DeleteUser(string userId)
        {
            var user = await FindByEmailForEdit(userId);
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

        public async Task<bool> ChangePassword(int id, string currentPasswordHash, string newPasswordHash)
        {
            var user = await GetDbSet<User>().SingleOrDefaultAsync(u => u.Id == id);
            if (user == null || user.Password != currentPasswordHash)
                return false;

            user.Password = newPasswordHash;
            GetDbSet<User>().Update(user);

            await UnitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task UpdatePassword(User user)
        {
            GetDbSet<User>().Update(user);
            await UnitOfWork.SaveChangesAsync();
        }
    }
}
