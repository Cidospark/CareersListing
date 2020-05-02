using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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



        // Dashboard
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }
        //--------------------------------------------------------------------------------------------------------


        // 
        [HttpGet]

        //--------------------------------------------------------------------------------------------------------


        // Manage claims users
        [HttpGet]
        public IActionResult ClaimsList()
        {
            var list = new List<ClaimsStoreViewModel>();
            foreach (var claim in ClaimsStore.AllCliams)
            {
                var claimsList = new ClaimsStoreViewModel()
                {
                    ClaimType = claim.Type
                };
                list.Add(claimsList);
            }
            return View(list);
        }
        [HttpGet]
        public async Task<IActionResult> ManageClaimsUsers(string type)
        {
            if (String.IsNullOrEmpty(type))
            {
                ViewBag.ErrorMessage = "Claim type is null or enpty";
                return RedirectToAction("NotFound");
            }
            // create a container object to hold the list of claims users
            var list = new List<ClaimsUsersViewModel>();


            // GET ALL USERS
            foreach (var user in _userManager.Users)
            {
                // create a user object and map Id, username values of the current user to it
                var claimUser = new ClaimsUsersViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                // get all the claims of this user
                var userClaims  = await _userManager.GetClaimsAsync(user);

                // check for a match
                for(int i=0; i < userClaims.Count; i++)
                {
                    if(userClaims[i].Type == type)
                    {
                        claimUser.IsSelected = true;
                        break;
                    }
                }

                list.Add(claimUser);

            }
            
            ViewBag.ClaimType = type;
            return View(list);
        }
        [HttpPost]
        public async Task<IActionResult> ManageClaimsUsers(List<ClaimsUsersViewModel> model, string type)
        {
            if (String.IsNullOrEmpty(type))
            {
                ViewBag.ErrorMessage = "Claim type is null or enpty";
                return RedirectToAction("NotFound");
            }

            // create a container object to hold the list of claims users
            var list = new List<ClaimsUsersViewModel>();


            foreach (var record in model)
            {
                var user = _userManager.Users.FirstOrDefault(u => u.Id == record.UserId);
                if(user == null)
                {
                    continue;
                }
                else
                {
                    var listClaim = new ClaimsUsersViewModel
                    {
                        UserId = record.UserId,
                        UserName = record.UserName
                    };

                    IdentityResult result = null;
                    // add all current selected claims
                    if (record.IsSelected)
                    {
                        result = await _userManager.AddClaimAsync(user, new Claim(type, "true"));
                        listClaim.IsSelected = true;
                        // TODO : throw error if the above operation fails
                    }
                    // remove all current unselected claims
                    else
                    {
                        result = await _userManager.RemoveClaimAsync(user, new Claim(type, "true"));
                        listClaim.IsSelected = false;
                        // TODO : throw error if the above operation fails
                    }

                    if (result.Succeeded)
                    {
                        list.Add(listClaim);
                    }

                }
            }

            return View(list);
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
                        // TODO : throw error if the above operation fails

                    }// if user is not select and is in role, remove user from role
                    else if(!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name)){
                        await _userManager.RemoveFromRoleAsync(user, role.Name);
                        // TODO : throw error if the above operation fails

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
