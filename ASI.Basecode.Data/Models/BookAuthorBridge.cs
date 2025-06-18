using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class BookAuthorBridge
    {
        [Key]
        public string BookAuthorBridgeId { get; set; }
        public string BookId { get; set; }
        public string AuthorId { get; set; }
    }
}
