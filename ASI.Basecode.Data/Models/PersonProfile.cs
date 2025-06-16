using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class PersonProfile
    {
        [Key]
        public string ProfileID { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Location { get; set; }
        public string? Role { get; set; }

        public string? AboutMe { get; set; }
    }
}
