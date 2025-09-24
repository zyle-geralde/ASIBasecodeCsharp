using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class ChangePasswordViewModel
    {
        public int UserId { get; set; }

        [Required, DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "Password must be at least 8 characters.", MinimumLength = 8)]
        [RegularExpression(
             @"^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$",
             ErrorMessage = "Password must be at least 8 characters and contain at least one uppercase letter, one number and one special character."
         )]

        public string NewPassword { get; set; }

        [Required, Compare("NewPassword", ErrorMessage = "Passwords don't match"), DataType(DataType.Password),]
        public string ConfirmPassword { get; set; }
    }
}
