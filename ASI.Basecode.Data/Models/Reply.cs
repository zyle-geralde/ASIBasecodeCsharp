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
        public string UserId { get; set; }
        public string AdminId { get; set; }
        public DateTime? UploadDate { get; set; }

        public string ReviewImage { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
