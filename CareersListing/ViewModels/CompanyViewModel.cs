using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.ViewModels
{
    public class CompanyViewModel
    {

        //public int Id { get; set; }

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

        [Display(Name = "Certificate PDF")]
        public IFormFile CompanyCertificate { get; set; }
        public string ExistingCertificate { get; set; }

        [Display(Name = "Logo")]
        public IFormFile Logo { get; set; }
        public string ExistingLogo { get; set; }

        public DateTime DateRegistered { get; set; }

        public CompanyViewModel()
        {
            DateRegistered = DateTime.Now;
        }
    }
}
