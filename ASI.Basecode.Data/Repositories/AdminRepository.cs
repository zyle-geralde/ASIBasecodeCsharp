using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class AdminRepository : BaseRepository, IAdminRepository
    {
        public AdminRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public bool AdminExists(string userId)
        {
            return this.GetDbSet<Admin>().Any(x => x.Email == userId);
        }

        public async Task AddAdmin(Admin user)
        {
            GetDbSet<Admin>().Add(user);
            await UnitOfWork.SaveChangesAsync();
        }
    }
}
