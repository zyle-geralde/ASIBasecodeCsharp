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
    }
}
