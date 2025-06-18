using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class PersonProfileRepository: BaseRepository, IPersonProfileRepository
    {
        public PersonProfileRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public async Task AddPersonProfile(PersonProfile personProfile)
        {
            this.GetDbSet<PersonProfile>().Add(personProfile);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<PersonProfile> GetPersonProfile(string username)
        {
            return await this.GetDbSet<PersonProfile>().FirstOrDefaultAsync(p => p.ProfileID == username);
        }

        public async Task EditPersonProfile(PersonProfile personProfile)
        {
            PersonProfile existingProfile = await GetPersonProfile(personProfile.ProfileID);
            if (existingProfile == null) return;
            existingProfile.FirstName = personProfile.FirstName;
            existingProfile.LastName = personProfile.LastName;
            existingProfile.MiddleName = personProfile.MiddleName;
            existingProfile.LastName = personProfile.LastName;
            existingProfile.Suffix = personProfile.Suffix;
            existingProfile.Gender = personProfile.Gender;
            existingProfile.ProfilePicture = personProfile.ProfilePicture;
            existingProfile.BirthDate = personProfile.BirthDate;
            existingProfile.Location = personProfile.Location;
            existingProfile.Role = personProfile.Role;
            existingProfile.AboutMe = personProfile.AboutMe;
            await UnitOfWork.SaveChangesAsync();
        }
    }
}
