using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class PersonProfileViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [StringLength(100, ErrorMessage = "About me must be less than 100 characters")]

        public string FirstName { get; set; }
        [StringLength(100, ErrorMessage = "About me must be less than 100 characters")]

        public string MiddleName { get; set; }
        [StringLength(100, ErrorMessage = "About me must be less than 100 characters")]

        public string LastName { get; set; }
        [StringLength(500, ErrorMessage = "About me must be less than 500 characters")]

        public string AboutMe { get; set; }
        public DateTime? Birthdate { get; set; }
        public string Gender { get; set; }
        [StringLength(250, ErrorMessage = "About me must be less than 100 characters")]

        public string Location { get; set; }
        public string ProfilePicture { get; set; }
        public List<ReviewViewModel> Reviews { get; set; } = new();
        [Required(ErrorMessage = "UserName is required.")]
        [StringLength(30, ErrorMessage = "Username must be between 3-30 characters", MinimumLength = 3)]
        public string Username { get; set; }
        public string Email { get; set; }
        public ChangePasswordViewModel ChangePassword { get; set; }

    }
}
