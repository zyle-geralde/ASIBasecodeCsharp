using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class PersonProfileRepository : BaseRepository, IPersonProfileRepository
    {
        public PersonProfileRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public async Task AddProfile(PersonProfile profile)
        {
            await this.GetDbSet<PersonProfile>().AddAsync(profile);
        }

        //public async Task EditProfile(PersonProfile profile)
        //{
        //    await this.GetDbSet<PersonProfile>().Update(profile);
        //    //await this.GetDbSet<PersonProfile>().Update(profile);
        //}
    }
}
