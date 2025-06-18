using ASI.Basecode.Data.Models;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IPersonProfileService
    {
        Task AddPersonProfile(PersonProfile personProfile);
        Task<bool> EditPersonProfile(PersonProfile personProfile);
        Task<PersonProfile> GetPersonProfile(string username);
    }
}
