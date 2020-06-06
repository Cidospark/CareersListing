using CareersListing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.ViewModels
{
    public class JobViewModel
    {
        public int Id { get; set; }
        public Company Company { get; set; }
        public int CompanyId { get; set; }
        public ApplicationUser Employer { get; set; }
        public string EmployerId { get; set; }

        public string JobTitle { get; set; }

        public string JobFunction { get; set; }

        public string Industry { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }
        public string SalaryScale { get; set; }

        public string JobDuration { get; set; }

        public string applicationUrl { get; set; }

        public string DaysAgo { get; set; }
        public string DateExpired { get; set; }

    }
}
