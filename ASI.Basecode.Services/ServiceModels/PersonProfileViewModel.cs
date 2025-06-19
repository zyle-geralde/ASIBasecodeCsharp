using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class PersonProfileViewModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string AboutMe { get; set; }
        public DateTime? Birthdate { get; set; }
        public string Gender { get; set; }
        public string Location { get; set; }
        public string ProfilePicture { get; set; }

    }
}
