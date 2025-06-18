using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class PersonProfileService: IPersonProfileService
    {
        private readonly IPersonProfileRepository _repository;
        private readonly IMapper _mapper;

        public PersonProfileService(IPersonProfileRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task AddPersonProfile(PersonProfile personProfile)
        {

            if (personProfile == null)
                throw new ArgumentNullException(nameof(personProfile));
            if (string.IsNullOrWhiteSpace(personProfile.ProfileID))
                throw new ArgumentException("ProfileID must be set", nameof(personProfile.ProfileID));

            await _repository.AddPersonProfile(personProfile);
        }
        public async Task<bool> EditPersonProfile(PersonProfile personProfile)
        {
            if(personProfile == null)
            {
                throw new ArgumentNullException(nameof(personProfile), "Person profile cannot be null");
            }

            var existingProfile = await _repository.GetPersonProfile(personProfile.ProfileID);
            if(existingProfile == null)
            {
                return false;
            }

            _mapper.Map(personProfile, existingProfile);

            await _repository.EditPersonProfile(existingProfile);
            return true;
        }

        public async Task<PersonProfile> GetPersonProfile(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("No username provided", nameof(username));
            }

            var profile = await _repository.GetPersonProfile(username);
            if (profile == null)
            {
                throw new KeyNotFoundException($"No profile found for username: {username}");
            }

            return profile;
        }
    }

}
