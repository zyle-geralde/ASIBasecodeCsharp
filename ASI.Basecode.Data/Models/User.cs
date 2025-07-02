using ASI.Basecode.Data.Classes;
using System;

namespace ASI.Basecode.Data.Models
{
    public partial class User : Person
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string OtpCode { get; set; }
        public DateTime? OtpExpirationDate { get; set; }
        public bool IsEmailVerified { get; set; } = false;
    }
}
