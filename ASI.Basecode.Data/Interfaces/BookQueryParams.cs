﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public class BookQueryParams
    {
        public string? SearchTerm { get; set; }
        public string? SearchAuhtor {  get; set; }

        public List<string>? GenreNames { get; set; } = new();

        public string? Author { get; set; }
        public int? Rating { get; set; }
        public DateTime? PublishedFrom { get; set; }
        public DateTime? PublishedTo { get; set; }
        public DateTime? UploadDate { get; set; }


        public string? Language { get; set; }
        public string? Publisher { get; set; }

        public string SortOrder { get; set; } = "title";
        public bool SortDescending { get; set; } = false;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsFeatured { get; set; }
    }
}
