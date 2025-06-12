using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class Reply
    {
        [Key]
        public string ReplyId { get; set; } 
        public string ReviewId { get; set; } 
        public string ReplyContent { get; set; }
        public int Likes { get; set; }
    }
}
