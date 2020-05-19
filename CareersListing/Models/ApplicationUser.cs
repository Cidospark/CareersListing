using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }
        
        public string Street { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "City must not be more than 100 characters")]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }
        public string Photo { get; set; }

        [Required]
        public DateTime DateRegistered { get; set; }

        public ICollection<JobApplication> JobApplication { get; set; }
        public ICollection<Company> Company { get; set; }

        public ApplicationUser()
        {
            DateRegistered = DateTime.Now;
        }
    }
}
