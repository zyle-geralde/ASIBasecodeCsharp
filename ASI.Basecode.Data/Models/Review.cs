﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class Review
    {
        [Key]
        public string ReviewId { get; set; } 
        public string BookId { get; set; }
        public float Rating { get; set; }
        public string Comment { get; set; }
        public int Likes { get; set; }
        public string UserId { get; set; }

        public DateTime? UploadDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}
