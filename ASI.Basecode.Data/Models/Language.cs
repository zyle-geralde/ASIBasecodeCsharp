using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class Language
    {
        [Key]
        public string LanguageId { get; set; }
        public string LanguageName { get; set; }
        public string AdminId { get; set; }
        public DateTime? UploadDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedByAdminId { get; set; }
    }
}
