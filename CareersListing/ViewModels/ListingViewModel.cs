using CareersListing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.ViewModels
{
    public class ListingViewModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public string DaysAgo { get; set; }
        public string JobTitle { get; set; }
        public string Industry { get; set; }
        public string Duration { get; set; }
        public string Location { get; set; }
    }
}
