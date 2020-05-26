using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Models
{
    public class Company
    {
        public int Id { get; set; }
        public ApplicationUser Employer { get; set; }
        public string EmployerId { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Company name must not be more than 100 characters")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Url]
        public string Website { get; set; }

        public string Street { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "City must not be more than 100 characters")]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }
        public string CompanyCertificate { get; set; }
        public string Logo { get; set; }

        [Required]
        public DateTime DateRegistered { get; set; }

        public Vacancy Vacancies { get; set; }

    }
}
