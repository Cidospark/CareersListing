using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareersListing.Models;
using CareersListing.Utilities;
using CareersListing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CareersListing.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // Reset Password (GET)
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }

            if (email == null)
            {
                ModelState.AddModelError("", "Invalid password reset email");
            }
            ViewBag.Email = email;
            return View();
        }
        // Reset Password (POST) -------------------------------------------------------------------------
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // confirm email is valid
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // use the user, token, and new password to reset the password
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                    if (result.Succeeded)
                    {
                        // if user account is locked out
                        // set the lockout end time to now
                        if(await _userManager.IsLockedOutAsync(user))
                        {
                            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }
                        return View("ResetPasswordConfirmation");
                    }

                    // if the above operation failed the add errors to model state
                    foreach(var err in result.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                    return View(model);
                }

                // if user is null goto confirmation page
                return View("ResetPasswordConfirmation");
            }
            return View(model);
        }
        //--------------------------------------------------------------------------------------------------------


        // Forgot Password (GET)
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        // Forgot Password (POST) -------------------------------------------------------------------------
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // confirm if email exist
                // Use email to get user to generate password reset token
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user != null && !await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Use user to generate token
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // Use token to genetate password reset link
                    _logger.LogWarning(Url.Action("ResetPassword", "Account", new { email = model.Email, token }, Request.Scheme));

                    return View("ForgotPasswordConfirmation");
                }

                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }
        //--------------------------------------------------------------------------------------------------------


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
                 PhoneNumber = user.PhoneNumber,
                AccountTtpe = await Utils.getUserAccountType(_userManager, user),
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
            ViewBag.PasswordErr = "Invalid Password!";
            
            var user = await _userManager.FindByIdAsync(model.Id);
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
            model.AccountTtpe = await Utils.getUserAccountType(_userManager, user);
            model.Country = user.Country;

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
        public IActionResult Register(string accType)
        {
            if(accType != "applicant" && accType != "employer")
            {
                ViewBag.accType = "Invalid account type!";
                return View();
            }
           
            ViewBag.accType = accType;
            return View();
        }

        // Register (POST)
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model, string acctype)
        {
            if (ModelState.IsValid)
            {
                // create a new user from values from model
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    City = model.City,
                    Country = model.Country,
                    Email = model.Email
                };

                // create user using user manager of the identity class
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // add role to user
                    if (acctype == "applicant")
                    {
                        await _userManager.AddToRoleAsync(user, "Applicant");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Employer");
                    }

                    // generate email confirmation email and log it
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { id = user.Id, token }, Request.Scheme);
                    _logger.LogWarning(confirmationLink);

                    // display a confimation message to user
                    ViewBag.ErrorTitle = "REGISTRATION WAS SUCCESSFUL!"; 
                    ViewBag.Message = "Please activate your account from the link sent to your email, Thank you.";
                    return View();
                }
                
                // display error if not successful
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            // go back to thesame view
            return View(model);
        }
        //--------------------------------------------------------------------------------------------------------


        // Confirm Email (GET)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string id, string token)
        {
           // if id or token is null, end the process
           if(id == null || token == null)
            {
                ViewBag.ErrorTitle = "Invalid Id or token";
                ViewBag.ErrorMessage = $"User or token cannot be null";
                return View("Error");
            }

            // ensure that user exist
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with id {id} cannot be found!";
                return View("NotFound");
            }

            // confirm email
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }

            // on failure
            ViewBag.ErrorTitle = "Conirmation Fialed";
            ViewBag.ErrorMessage = $"Could not confirm email.";
            return View("Error");

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
