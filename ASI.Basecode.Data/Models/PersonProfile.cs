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
     
        public User? User { get; set; }
        public string? FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Suffix { get; set; } = string.Empty;
        public string? Gender { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; } = null;
        public string? Location { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string AboutMe { get; set; } = string.Empty;
    }
}
