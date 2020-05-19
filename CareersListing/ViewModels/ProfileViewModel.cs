using CareersListing.Models;
using CareersListing.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.ViewModels
{
    public class ProfileViewModel
    {
        public ProfileViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }

        public string Id { get; set; }

        [Required(ErrorMessage = "Lastname required")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Firstname require")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        public string AccountTtpe { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Street { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "City must not be more than 100 characters")]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        [Display(Name = "Photo")]
        public IFormFile FormPhoto { get; set; }
        public string ExistingPhotoPath { get; set; }

        public List<string> Claims { get; set; }
        public IList<string> Roles { get; set; }
        public DateTime DateRegistered { get; set; }

    }
}
