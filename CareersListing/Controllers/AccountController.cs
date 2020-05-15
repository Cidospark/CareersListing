using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareersListing.Models;
using CareersListing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CareersListing.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ChangePasswordConfirmation (GET)
        [HttpGet]
        public IActionResult ChangePasswordConfirmation()
        {
            return View();
        }
        // --------------------------------------------------------------------------------------------------

        // Edit Password (GET)
        [HttpGet]
        public async Task<IActionResult> ChangePassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new ChangePasswordViewModel{
                 LastName = user.LastName,
                 FirstName = user.FirstName,
                 AccountType = user.AccountType,
                 PhoneNumber = user.PhoneNumber,
                 Email = user.Email,
                 City = user.City,
                 Country = user.Country,
                 ExistingPhotoPath = user.Photo
            };

            return View(model);
        }
        // Edit Password (POST) -------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            ApplicationUser user = null;
            ViewBag.PasswordErr = "Invalid Password!";
            
            user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {model.Id} cannot be found";
                return View("NotFound");
            }

            model.ExistingPhotoPath = user.Photo;
            model.LastName = user.LastName;
            model.FirstName = user.FirstName;
            model.PhoneNumber = user.PhoneNumber;
            model.City = user.City;
            model.Email = user.Email;
            model.Country = user.Country;
            model.AccountType = user.AccountType;

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction("ChangePasswordConfirmation", "Account");
            }
                
            foreach(var err in result.Errors)
            {
                ModelState.AddModelError("", err.Description);
                ViewBag.PasswordErr = "Failed to change password!";
            }

            return View(model);
        }
        //--------------------------------------------------------------------------------------------------------

        // Login (GET)
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Dashboard", "Administration");
            }
            return View();
        }

        // Login (POST)
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user == null)
                {
                    ModelState.AddModelError(String.Empty, "Email not confirmed yet");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!String.IsNullOrEmpty(ReturnUrl))
                    {
                        return LocalRedirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Dashboard","Administration");
                    }
                }

                //if (result.IsLockedOut)
                //{
                //    return View("AccountLockedout");
                //}

                ModelState.AddModelError("", "Invalid Attempt");

            }
            return View(model);
        }
        //--------------------------------------------------------------------------------------------------------


        // Register User (GET)
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // Register (POST)
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
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
                    if(model.AccountType == AccountType.Applicant)
                    {
                        await _userManager.AddToRoleAsync(user, "Applicant");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Employer");
                    }

                    ViewBag.ErrorTitle = "REGISTRATION WAS SUCCESSFUL!"; 
                    ViewBag.Message = "Please activate your account from the link sent to your email, Thank you.";
                    return View();
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        //--------------------------------------------------------------------------------------------------------

        // Check if email already exists (GET, POST)
        [AcceptVerbs("Get","Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} already exists!");
            }
        }
        //--------------------------------------------------------------------------------------------------------


        // Logout (GET)
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        //--------------------------------------------------------------------------------------------------------


    }
}
