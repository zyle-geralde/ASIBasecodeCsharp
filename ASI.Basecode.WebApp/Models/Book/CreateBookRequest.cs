using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Models.Book

{
    public class CreateBookRequest
    {
        [StringLength(500)]
        public string BookId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(100)]
        public string? Subtitle { get; set; }

        [DataType(DataType.Date)]
        public string? UploadDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? UpdatedDate { get; set; }

        [DataType(DataType.Date)]
        public string? PublicationDate { get; set; }

        public string? Publisher { get; set; }

        public string? PublicationLocation { get; set; } 

        [StringLength(10000)]
        public string? Description { get; set; }

        public int? NumberOfPages { get; set; }

        [StringLength(20)]
        public string? Language { get; set; }

        public string? CoverImageUrl { get; set; }
        
        public string? BookFileUrl { get; set; }

        [StringLength(100)]
        public string? SeriesName { get; set; }

        public int? SeriesOrder { get; set; }

        [StringLength(500)]
        public string? SeriesDescription { get; set; }

        public float? AverageRating { get; set; }

        public string? Author { get; set; }

        public int? Likes { get; set; }

        [StringLength(100)]
        public string? ISBN10 { get; set; }

        [StringLength(100)]
        public string? ISBN13 { get; set; }

        [StringLength(100)]
        public string? Edition { get; set; }
        public string AdminId { get; set; }
    }
}
