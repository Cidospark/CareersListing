using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CareersListing.Models;
using Microsoft.AspNetCore.Mvc;
using CareersListing.Utilities;

namespace CareersListing.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Lastname required")]
        [Display(Name = "Last name : ")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Firstname require")]
        [Display(Name = "First name : ")]
        public string FirstName { get; set; }
        [Required]
        public AccountType AccountType { get; set; }
        [Required(ErrorMessage = "Email required")]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "Account")]
        [ValidEmailDomain(allowedDomain: "sample.com", ErrorMessage = "Email domain must be 'sample.com'")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation password does not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "City must not be more than 100 characters")]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

    }
}
