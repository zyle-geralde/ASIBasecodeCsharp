using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class BookGenreBridge
    {
        [Key]
        public string BookGenreBridgeId { get; set; }
        public string BookId { get; set; } 
        public string BookGenreId { get; set; } 
    }
}
