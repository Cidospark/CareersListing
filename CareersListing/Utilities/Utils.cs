using CareersListing.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Utilities
{
    public static class Utils
    {
        public static string UploadFile(IFormFile file, string wwwRootPath)
        {

            // get a unique name
            string filename = Guid.NewGuid().ToString() + "_" + file.FileName;

            // combine name and path
            var fullPath = Path.Combine(wwwRootPath, filename);

            // copy
            using(var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            
            return filename;
        }

        public static async Task<string> getUserAccountType(UserManager<ApplicationUser> userManager, ApplicationUser user)
        {
            string userAccType = null;

            if (await  userManager.IsInRoleAsync(user, "APPLICANT"))
            {
                userAccType += "Applicant, ";
            }

            if (await userManager.IsInRoleAsync(user, "EMPLOYER"))
            {
                userAccType += "Employer, ";
            }

            if (await userManager.IsInRoleAsync(user, "ADMIN"))
            {
                userAccType += "Admin, ";
            }

            if (await userManager.IsInRoleAsync(user, "SUPER ADMIN"))
            {
                userAccType += "Super Admin, ";
            }

            return userAccType;
        }
    }
}
