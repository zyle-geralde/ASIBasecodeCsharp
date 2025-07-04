﻿using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IPersonProfileRepository
    {
        Task AddPersonProfile(PersonProfile personProfile);
        Task EditPersonProfile(PersonProfile personProfile);
        Task DeletePersonProfile(string profileId);
        Task<PersonProfile> GetPersonProfile(string username);
    }
}
