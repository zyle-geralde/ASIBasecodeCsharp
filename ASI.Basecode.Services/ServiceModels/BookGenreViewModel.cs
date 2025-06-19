using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class BookGenreViewModel
    {
        [Required]
        [StringLength(200)]
        public string GenreName { get; set; }
        [StringLength(500)]
        public string? GenreDescription { get; set; }

        [StringLength(1000)]
        public string? GenreImageUrl { get; set; }

        public string? AdminId { get; set; }
        public DateTime? UploadDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedByAdminId { get; set; }
    }
}
