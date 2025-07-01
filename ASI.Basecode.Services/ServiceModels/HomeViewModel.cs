using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class HomeViewModel
    {
        public List<BookGenreViewModel> Genres { get; set; }
        public List<BookViewModel> TopRatedBooks { get; set; }
        public List<BookViewModel> NewBooks { get; set; }
        public PaginatedList<BookViewModel> SearchResults { get; set; }
        public string? SearchTerm { get; set; }
        public int PageIndex { get; set; }
        public string SortOrder { get; set; }
        public bool SortDescending { get; set; }
        public int PageSize { get; set; }
    }
}
