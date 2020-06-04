using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.ViewModels
{
    public class HomeViewModel
    {
        public List<ListingViewModel> Listings { get; set; }
        public int NumberOfListings { get; set; }
        public int NumberOfRegisteredCompanies { get; set; }
        public HomeViewModel()
        {
            Listings = new List<ListingViewModel>();
        }
    }
}
