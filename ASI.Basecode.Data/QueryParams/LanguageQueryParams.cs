using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.QueryParams
{
    public class LanguageQueryParams
    {
        public string? SearchTerm { get; set; }
        public string SortOrder { get; set; } = "Name";
        public bool SortDescending { get; set; } = false;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
