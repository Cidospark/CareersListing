using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Models
{
    public class JobPost
    {
        public int Id { get; set; }
        public Company Company { get; set; }
        public int CompanyId { get; set; }

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
        public double SalaryScale { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "Job duration must not be more than 20 characters")]
        public double JobDuration { get; set; }

        [Required]
        public DateTime DatePosted { get; set; }

        [Required]
        public DateTime DateExpired { get; set; }

        public ICollection<JobApplication> JobApplication { get; set; }

    }
}
