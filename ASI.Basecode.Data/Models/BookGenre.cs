using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class BookGenre
    {
        [Key]
        public string BookGenreId { get; set; }
        public string GenreName { get; set; }
        public string? GenreDescription { get; set; }
        public string AdminId { get; set; }
        public DateTime? UploadDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedByAdminId { get; set; }
    }
}
