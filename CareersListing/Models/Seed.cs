using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Models
{
    public class Seed
    {
        public static async Task SeedUsers(ApplicationDbContext context,
                                           UserManager<ApplicationUser> userManager,
                                           RoleManager<IdentityRole> roleManager,
                                           ILogger logger)
        {
            context.Database.EnsureCreated();

            if (!roleManager.Roles.Any())
            {
                var roles = new List<IdentityRole>
                {
                    new IdentityRole {Name = "Super Admin"},
                    new IdentityRole {Name = "Admin"},
                    new IdentityRole {Name = "Applicant"},
                    new IdentityRole {Name = "Employer"},
                };

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }

            }

            if (userManager.Users.Any())
            {
                // construct default user
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@sample.com",
                    Email = "admin@sample.com",
                    LastName = "John",
                    FirstName = "Doe",
                    City = "London",
                    Country = "England"
                };
                try
                {
                    var result = await userManager.CreateAsync(adminUser, "P@$$w0rd1"); 
                    if (result.Succeeded)
                    {
                        await userManager.AddToRolesAsync(adminUser, new[] {"Super Admin","Admin"});
                    }
                }
                 catch (Exception e)
                {
                    logger.LogError(e.InnerException.Message);
                }

               
            }
        }
    }
}
