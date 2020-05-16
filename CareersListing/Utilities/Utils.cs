using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
    }
}
