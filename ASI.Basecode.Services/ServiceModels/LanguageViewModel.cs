using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class LanguageViewModel
    {
        [StringLength(500)]
        public string? LanguageId { get; set; }

        [Required]
        [StringLength(300)]
        public string? LanguageName { get; set; }

        [StringLength(300)]
        public string? CreatedBy { get; set; }

        [DataType(DataType.Date)]
        public DateTime? UploadDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? UpdatedDate { get; set; }

        [StringLength(300)]
        public string? UpdatedBy { get; set; }
    }
}
