using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.ViewModels
{
    public class ListCompaniesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }
        
        public string City { get; set; }

        public string Country { get; set; }
    }
}
