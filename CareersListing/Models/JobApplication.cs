using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Models
{
    public class JobApplication
    {
        public int Id { get; set; }
        public ApplicationUser Applicant { get; set; }
        public string ApplicantId { get; set; }
        public Vacancy Vacancy { get; set; }
        public int VacancyId { get; set; }
        public string Description { get; set; }

        [Required]
        public DateTime DateRegistered { get; set; }

    }
}
