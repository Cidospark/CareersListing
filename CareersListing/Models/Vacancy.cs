using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Models
{
    public class Vacancy
    {
        public int Id { get; set; }
        public Company Company { get; set; }
        public int CompanyId { get; set; }
        public ApplicationUser Employer { get; set; }
        public string EmployerId { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Job title must not be more than 100 characters")]
        public string JobTitle { get; set; }

        [Required]
        public string JobFunction { get; set; }

        [Required]
        public string Industry { get; set; }

        [Required]
        public string Location { get; set; }

        public string Description { get; set; }
        public string SalaryScale { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "Job duration must not be more than 20 characters")]
        public string JobDuration { get; set; }

        [Required]
        public string applicationUrl { get; set; }

        [Required]
        public DateTime DatePosted { get; set; }

        [Required]
        public DateTime DateExpired { get; set; }


    }
}
