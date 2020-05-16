using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CareersListing.Models;
using CareersListing.Utilities;
using CareersListing.ViewModels;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IHostingEnvironment _hostingEnvironment;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager,
                                        IHostingEnvironment hostingEnvironment)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }
        // -------------------------------------------------------- 


        public IActionResult Index()
        {
            return RedirectToAction("Dashboard", "Administration");
        }
        // -------------------------------------------------------- 



        // Dashboard (GET)
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }
        //--------------------------------------------------------------------------------------------------------


        // Profile (GET) 
        [HttpGet]
        public async Task<IActionResult> Profile(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id : {Id} was not found";
                return View("NotFound");
            }


            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);


            var model = new ProfileViewModel
            {
                Id = user.Id,
                LastName = user.LastName,
                FirstName = user.FirstName,
                AccountType = user.AccountType,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Street = user.Street,
                City = user.City,
                Country = user.Country,
                ExistingPhotoPath = user.Photo,
                DateRegistered = user.DateRegistered,
                Claims = userClaims.Where(x => x.Value == "true").Select(c => c.Type).ToList(),
                Roles = userRoles
            };

            return View(model);
        }

        // Profile (POST) ---------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFilename = null;
                
                // if photo is selected
                if(model.FormPhoto != null)
                {
                    var hostingEnvPath = _hostingEnvironment.WebRootPath+"/images/profile";
                    // if there is an existing photo
                    if (model.ExistingPhotoPath != null)
                    {
                        // get the path to the wwwroot folder combined with file name, then delete it
                        var fullPath = Path.Combine(hostingEnvPath, model.ExistingPhotoPath);
                        System.IO.File.Delete(fullPath);
                    }

                    uniqueFilename = Utils.UploadFile(model.FormPhoto, hostingEnvPath);

                }
                else
                {
                    uniqueFilename = model.ExistingPhotoPath;
                }

                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    ViewBag.ErrorMessage = $"User with id : {model.Id} was not found!";
                    return View("NotFound");
                }

                user.LastName = model.LastName;
                user.FirstName = model.FirstName;
                user.PhoneNumber = model.PhoneNumber;
                user.Street = model.Street;
                user.City = model.City;
                user.Country = model.Country;
                user.Photo = uniqueFilename;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Profile","Administration", new { id = model.Id});
                }

                foreach(var err in result.Errors)
                {
                    ModelState.AddModelError("",err.Description);
                }
            }
            return View(model);

        }
        //--------------------------------------------------------------------------------------------------------


        // List of claims (GET) 
        [HttpGet]
        public IActionResult ClaimsList()
        {
            var list = new List<ClaimsListViewModel>();
            foreach (var claim in ClaimsStore.AllCliams)
            {
                var claimsList = new ClaimsListViewModel()
                {
                    ClaimType = claim.Type
                };
                list.Add(claimsList);
            }
            return View(list);
        }

        // Manage claims users (POST) -------------------------------------------------------------------------
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

        // Manage claims users (POST) -------------------------------------------------------------------------
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

        //--------------------------------------------------------------------------------------------------------


        // Manage user claims (GET)
        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            // get user by Id
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {userId} cannot be found";
                return View("NotFound");
            }

            // get user's existing claims
            var existingUserClaim = await _userManager.GetClaimsAsync(user);
            var existingRoleClaim = await _userManager.GetRolesAsync(user);
            var model = new ManageUsersClaimsViewModel
            {
                Id = user.Id,
                LastName = user.LastName,
                FirstName = user.FirstName,
                AccountType = user.AccountType,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Street = user.Street,
                City = user.City,
                Country = user.Country,
                ExistingPhotoPath = user.Photo,
                DateRegistered = user.DateRegistered,
                ListOfUserRoles = existingRoleClaim
            };

            foreach(var claim in ClaimsStore.AllCliams)
            {
                var eachClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };

                if(existingUserClaim.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    eachClaim.IsSelected = true;
                }

                model.ListOfUserClaims.Add(eachClaim);
            }

            return View(model);
        }

        // Manage user claims (POST) ----------------------------------------------------- 
        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(ManageUsersClaimsViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {model.Id} cannot be found";
                return View("NotFound");
            }

            // if we have found the user
            // retrieve all the user claims and delete those claims
            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);
            // if there is any problem removing the claims then
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            // retrieve all selected claims
            result = await _userManager.AddClaimsAsync(user, 
                model.ListOfUserClaims.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }
            return RedirectToAction("Profile", "Administration", new { model.Id });
        }
        //--------------------------------------------------------------------------------------------------------


        // DeleteRole (POST) 
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
        // --------------------------------------------------------------------------------------------- 


        // Manage role users (GET) 
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

        // Manage role users (POST) -------------------------------------------------------------------------
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

            return RedirectToAction("ManageRoleUsers","Administration", new {roleId});
        }
        // -------------------------------------------------------- 


        // Create role (GET)
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

        // Create role (POST) -------------------------------------------------------------------------
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
        // ------------------------------------------------------------------------------------


        // List of Roles (GET)
        [HttpGet]
        public IActionResult Roles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }
        //--------------------------------------------------------------------------------------------------------


        // List of registered users (GET)
        [HttpGet]
        public IActionResult Users()
        {
            var users = _userManager.Users;
            return View(users);
        }
        //--------------------------------------------------------------------------------------------------------

        // Create user (GET)
        [HttpGet] 
        public IActionResult CreateUser()
        {
            return View();
        }

        // Create user (POST)
        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // create a new user from values from model
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    AccountType = model.AccountType,
                    City = model.City,
                    Country = model.Country,
                    Email = model.Email
                };

                // create user using user manager of the identity class
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (model.AccountType == AccountType.Applicant)
                    {
                        await _userManager.AddToRoleAsync(user, "Applicant");
                    }
                    else if(model.AccountType == AccountType.Employer)
                    {
                        await _userManager.AddToRoleAsync(user, "Employer");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }

                    ViewBag.RegMessage = "REGISTRATION WAS SUCCESSFUL!";
                    // TODO : send confirmation message to user email
                    
                    return RedirectToAction("Users","Administration");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        //--------------------------------------------------------------------------------------------------------


        // Delete user (POST)
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with id : {id} was not found!";
                return View("NotFound");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Users", "Administration");
            }

            foreach(var err in result.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }

            return View();
        }
        //--------------------------------------------------------------------------------------------------------
    }
}
