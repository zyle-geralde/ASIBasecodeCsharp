using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class BookViewModel
    {
        [StringLength(500)]
        public string BookId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(100)]
        public string? Subtitle { get; set; }
        public string? GenreList { get; set; }

        [DataType(DataType.Date)]
        public DateTime? UploadDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? UpdatedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PublicationDate { get; set; }

        [StringLength(100)]
        public string? Publisher { get; set; }

        [StringLength(100)]
        public string? PublicationLocation { get; set; }

        [StringLength(10000)]
        public string? Description { get; set; }

        public int? NumberOfPages { get; set; }

        [StringLength(500)]
        public string? Language { get; set; }

        [StringLength(1000)]
        public string? CoverImageUrl { get; set; }

        [StringLength(1000)]
        public string? BookFileUrl { get; set; }

        [StringLength(100)]
        public string? SeriesName { get; set; }

        public int? SeriesOrder { get; set; }

        [StringLength(500)]
        public string? SeriesDescription { get; set; }

        public float? AverageRating { get; set; }

        [StringLength(100)]
        public string? Author { get; set; }

        public int? Likes { get; set; }

        [StringLength(100)]
        public string? ISBN10 { get; set; }

        [StringLength(100)]
        public string? ISBN13 { get; set; }

        [StringLength(100)]
        public string? Edition { get; set; }
        public string CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public List<ReviewViewModel> Reviews { get; set; } = new();
        public int ReviewCount { get; set; }
        public bool HasReviewed { get; set; }
        public string? CoverImage { get; set; }
        public string? BookFile { get; set; }
        public bool? IsFeatured { get; set; } = false;
    }
}
