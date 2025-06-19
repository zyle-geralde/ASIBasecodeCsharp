using ASI.Basecode.Data.Models;
using System.Threading.Tasks;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IPersonProfileService
    {
        Task AddPersonProfile(PersonProfile personProfile);
        Task<bool> EditPersonProfile(PersonProfileViewModel personProfile);
        Task<PersonProfile> GetPersonProfile(string username);
    }
}
