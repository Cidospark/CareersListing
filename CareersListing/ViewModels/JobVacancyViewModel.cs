using CareersListing.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.ViewModels
{
    public class JobVacancyViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Required]
        [Display(Name ="Job Title")]
        [MaxLength(100, ErrorMessage = "Job title must not be more than 100 characters")]
        public string JobTitle { get; set; }

        [Required]
        [Display(Name = "Job Function")]
        public string JobFunction { get; set; }

        [Required]
        public string Industry { get; set; }

        [Required]
        public string Location { get; set; }

        public string Description { get; set; }

        [Display(Name = "Salary")]
        public string SalaryScale { get; set; }

        [Required]
        [Display(Name = "Duration")]
        [MaxLength(20, ErrorMessage = "Job duration must not be more than 20 characters")]
        public string JobDuration { get; set; }

        [Required]
        [Display(Name = "Application Form Url")]
        public string applicationUrl { get; set; }

        [Required]
        public DateTime DatePosted { get; set; }

        [Required]
        [Display(Name = "Expiry date")]
        public DateTime DateExpired { get; set; }

        public List<ListOfJobVacancies> Vacancies { get; set; }

        public JobVacancyViewModel()
        {
            DatePosted = DateTime.Now;
            Vacancies = new List<ListOfJobVacancies>();
        }
    }

    public class ListOfJobVacancies
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }

        public Company Company { get; set; }

        public string Salaries { get; set; }

        public DateTime DateExpired { get; set; }
        public string Duration { get; set; }
        public string Location { get; set; }

    }
}
