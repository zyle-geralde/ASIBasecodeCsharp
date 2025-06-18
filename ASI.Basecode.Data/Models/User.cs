using ASI.Basecode.Data.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Data.Models
{
    public partial class User:Person
    {
        public int Id { get; set; }
        [Key]
        public string UserId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string AdminId { get; set; }
        public PersonProfile? Profile { get; set; }
    }
}
