using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareersListing.Models;
using CareersListing.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CareersListing.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard", "Administration");
        }

        // Create role
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async  Task<IActionResult> CreateRole(CreateRoleViewModel model)
        { 
            // if model is valid
            if (ModelState.IsValid)
            {
                // create role object using value from model passed in
                var role = new IdentityRole()
                {
                    Name = model.RoleName
                };

                // create the role in the database  
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles", "Administration");
                }

            }

            // if model not valid
            return View(model);
        }
        // -------------------------------------------------------- 

        [HttpGet]
        public IActionResult Roles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }
    }
}
