using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Models
{
    public class Applicant
    {
        public int Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }
        public string CV { get; set; }

        public ICollection<JobApplication> JobApplication { get; set; }

    }
}
