using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class AuthorViewModel
    {
        [StringLength(450)]
        public string AuthorId { get; set; }

        [Required(ErrorMessage = "Author name is required.")]
        [StringLength(450)]
        public string AuthorName { get; set; }
        [StringLength(5000)]
        public string AuthorDescription { get; set; }
        [Required(ErrorMessage = "Author image is required.")]

        [StringLength(500)]
        public string AuthorImageUrl { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UploadDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
