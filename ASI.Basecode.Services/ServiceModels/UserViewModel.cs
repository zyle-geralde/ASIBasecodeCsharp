using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(30, ErrorMessage = "Username must be between 3-30 characters", MinimumLength = 3)]

        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "Password must be at least 8 characters.", MinimumLength = 8)]
        [DataType(DataType.Password)]

        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmation Password is required.")]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
        [DataType(DataType.Password)]

        public string ConfirmPassword { get; set; }

        public string? Role { get; set; }
        public string? OtpCode { get; set; }
        public DateTime? OtpExpirationDate { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        public int Id { get; set; }
        public bool IsUpdate { get; set; } = false;
    }
}
