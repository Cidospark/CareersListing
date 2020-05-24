using CareersListing.Models;
using CareersListing.ViewModels;
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
        public static string UploadFile(IFormFile FormPhoto, string ExistingPhotoPath, IHostingEnvironment hostingEnvironment, string folderName)
        {
            string uniqueFilename = null;

            if (FormPhoto != null)
            {
                var hostingEnvPath = hostingEnvironment.WebRootPath + "/images/"+ folderName;
                // if there is an existing photo
                if (ExistingPhotoPath != null)
                {
                    // get the path to the wwwroot folder combined with file name, then delete it
                    var fullPath = Path.Combine(hostingEnvPath, ExistingPhotoPath);
                    System.IO.File.Delete(fullPath);
                }

                //uniqueFilename = Utils.UploadFile(model.FormPhoto, hostingEnvPath);

                // get a unique name
                uniqueFilename = Guid.NewGuid().ToString() + "_" + FormPhoto.FileName;

                // combine name and path
                var path = Path.Combine(hostingEnvPath, uniqueFilename);

                // copy
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    FormPhoto.CopyTo(fileStream);
                }

            }
            else
            {
                uniqueFilename = ExistingPhotoPath;
            }
            
            return uniqueFilename;
        }

        public static void DeleteFile(string ExistingPhotoPath, IHostingEnvironment hostingEnvironment, string folderName)
        {
            var hostingEnvPath = hostingEnvironment.WebRootPath + "/images/" + folderName;
            // if there is an existing photo
            if (ExistingPhotoPath != null)
            {
                // get the path to the wwwroot folder combined with file name, then delete it
                var fullPath = Path.Combine(hostingEnvPath, ExistingPhotoPath);
                System.IO.File.Delete(fullPath);
            }
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
