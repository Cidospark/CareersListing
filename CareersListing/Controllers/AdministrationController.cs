using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareersListing.Models;
using CareersListing.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CareersListing.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        // -------------------------------------------------------- 

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard", "Administration");
        }
        // -------------------------------------------------------- 


        // DeleteRole
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role {role.Name} was not found!";
                return RedirectToAction("NotFound");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("Roles", "Administration");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return RedirectToAction ("Roles", "Administration");
        }
        // -------------------------------------------------------- 

        // Manage role users
        [HttpGet]
        public async Task<IActionResult> ManageRoleUsers(string roleId)
        {
            // get role by id
            var role = await _roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role {role.Name} was not found!";
                return RedirectToAction("NotFound");
            }

            // if role is not null
            // Create an object to hold a collection of users
            var roleUsers = new List<RoleUsersViewModel>();

            foreach(var user in _userManager.Users)
            {
                var roleUser = new RoleUsersViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                // if this user is in role the assign 'true' to isSelected else assign 'false'
                roleUser.IsSelected = await _userManager.IsInRoleAsync(user, role.Name)? true : false;

                roleUsers.Add(roleUser);

            }
            ViewBag.RoleID = role.Id;
            ViewBag.RoleName = role.Name;
            return View(roleUsers);
        }
        [HttpPost]
        public async Task<IActionResult> ManageRoleUsers(List<RoleUsersViewModel> model, string roleId)
        {
            // get role by id
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role {role.Name} was not found!";
                return RedirectToAction("NotFound");
            }

            // foreach user in the model
            for(int i=0; i < model.Count; i++)
            {
                // a user record
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == model[i].UserId);
                if(user == null)
                {
                    continue;
                }
                else
                {
                    // if user is selected and is not in role, add user to role
                    if(model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }// if user is not select and is in role, remove user from role
                    else if(!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name)){
                        await _userManager.RemoveFromRoleAsync(user, role.Name);
                    }// else continue
                    else
                    {
                        continue;
                    }
                }
            }

            return RedirectToAction("ManageRoleUsers","Administration", new { roleId = roleId});
        }
        // -------------------------------------------------------- 

        // Create role
        [HttpGet]
        public async Task<IActionResult> CreateRole(string Id)
        {
            if (!String.IsNullOrEmpty(Id))
            {
                var role = await _roleManager.FindByIdAsync(Id);
                var roleViewModel = new CreateRoleViewModel
                {
                    Id = role.Id,
                    RoleName = role.Name
                };
                ViewBag.PageTitle = "Edit Role";
                return View(roleViewModel);
            }
            return View();
        }
        [HttpPost]
        public async  Task<IActionResult> CreateRole(CreateRoleViewModel model)
        { 
            // if model is valid
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(model.Id))
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

                // if string id is not empty then edit role
                var edittedRole = await _roleManager.FindByIdAsync(model.Id);
                if(edittedRole == null)
                {
                    ViewBag.ErrorMessage = $"Role {model.RoleName} was not found!";
                    return RedirectToAction("NotFound");
                }

                // if role is found
                edittedRole.Id = model.Id;
                edittedRole.Name = model.RoleName;

                var resultOfEditted = await _roleManager.UpdateAsync(edittedRole);
                if (resultOfEditted.Succeeded)
                {
                    return RedirectToAction("Roles", "Administration");
                }

                foreach(var error in resultOfEditted.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);

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
