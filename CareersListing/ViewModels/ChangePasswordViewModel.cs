using CareersListing.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.ViewModels
{
    public class ChangePasswordViewModel
    {

        public string Id { get; set; }

        [Required(ErrorMessage = "Lastname required")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Firstname require")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string AccountTtpe { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "City must not be more than 100 characters")]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        [Display(Name = "Photo")]
        public string ExistingPhotoPath { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "New Password and Confirm Password does not match")]
        public string ConfirmPassword { get; set; }
    }
}
