using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class ChangePasswordViewModel
    {
        public int UserId { get; set; }

        [Required, DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required, DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password must be at least 8 characters.", MinimumLength = 8)]

        public string NewPassword { get; set; }

        [Required, Compare("NewPassword", ErrorMessage = "Passwords don't match"), DataType(DataType.Password),]
        public string ConfirmPassword { get; set; }
    }
}
