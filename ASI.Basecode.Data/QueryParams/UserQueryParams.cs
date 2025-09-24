using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.QueryParams
{
    public class UserQueryParams
    {
        public string SearchTerm { get; set; }
        public string Role { get; set; }
        public string SortOrder { get; set; } = "UpdatedDate";
        public bool SortDescending { get; set; } = true;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
