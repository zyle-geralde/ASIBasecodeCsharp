using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.ServiceModels
{
    public class AdminDashboardViewModel
    {
        public int TotalReviews { get; set; }
        public int TotalBooks { get; set; }
        public int TotalUsers { get; set; }
        public int TotalGenres { get; set; }
        public List<BookViewModel> TopRatedBooks { get; set; }
        public List<BookViewModel> NewBooks { get; set; }
        public List<BookViewModel> FeaturedBooks { get; set; }




    }
}
