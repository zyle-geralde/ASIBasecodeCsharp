using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class Book
    {
        public string BookId { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public DateTime? UploadDate { get; set; }
        public DateTime? PublicationDate { get; set; }
        public List<string>? Publisher { get; set; }
        public List<string>? PublicationLocation { get; set; }
        public string? Description { get; set; }
        public int? NumberOfPages { get; set; }
        public string? Language { get; set; }
        public string? CoverImage { get; set; }
        public string? BookFile { get; set; }
        public string? SeriesName { get; set; }
        public string? SeriesDescription { get; set; }
        public float? AverageRating { get; set; }
        public List<string>? Author { get; set; }
        public int? Likes { get; set; }
    }
}
