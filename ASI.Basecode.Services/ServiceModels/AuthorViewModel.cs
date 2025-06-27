using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class AuthorViewModel
    {
        [StringLength(450)]
        public string AuthorId { get; set; }

        [Required]
        [StringLength(450)]
        public string AuthorName { get; set; }
        [StringLength(5000)]
        public string AuthorDescription { get; set; }
        [StringLength(500)]
        public string AuthorImageUrl { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UploadDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
