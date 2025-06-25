using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class AuthorModel
    {
        [Key]
        [StringLength(450)]
        public string AuthorId { get; set; }

        public string AuthorName { get; set; }
        public string AuthorDescription { get; set; }
        public string AuthorImageUrl { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UploadDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
