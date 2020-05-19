using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.ViewModels
{
    public class UsersViewModel
    {
        public string Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string AccountTtpe { get; set; }
        public string Country { get; set; }
        public DateTime DateOfReg { get; set; }
    }
}
