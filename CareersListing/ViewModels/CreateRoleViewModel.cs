using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CareersListing.ViewModels
{
    public class CreateRoleViewModel
    {
        public string Id { get; set; }
        [Required]
        public string RoleName { get; set; }
        
    }
}
