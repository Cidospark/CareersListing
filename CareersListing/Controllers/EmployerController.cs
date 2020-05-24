using CareersListing.Models;
using CareersListing.Utilities;
using CareersListing.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Controllers
{
    public class EmployerController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ICompanyRepo _companyRepo;
        private readonly ILogger<EmployerController> _logger;

        public EmployerController(RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager,
                                        IHostingEnvironment hostingEnvironment,
                                        ICompanyRepo companyRepo, ILogger<EmployerController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _companyRepo = companyRepo;
            _logger = logger;
        }
        // ----------------------------------------------------------------------------------------

        // Company (GET)
        [HttpGet]
        public async Task<IActionResult> Company()
        {
            var companies = await _companyRepo.GetAllCompanies();
            CompanyViewModel model = new CompanyViewModel();
            foreach (var company in companies)
            {
                var row = new ListCompaniesViewModel
                {
                    Id = company.Id,
                    Name = company.Name,
                    Email = company.Email,
                    Website = company.Website,
                    City = company.City,
                    Country = company.Country
                };

                model.ListOfCompanies.Add(row);
            }

            return View(model);
        }
        // Company (POST) ---------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Company(CompanyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var company = new Company
                {
                    Name = model.Name,
                    Email = model.Email,
                    Website = model.Website,
                    Street = model.Street,
                    City = model.City,
                    Country = model.Country,
                    CompanyCertificate = Utils.UploadFile(model.CompanyCertificate, model.ExistingCertificate, _hostingEnvironment, "Certificates"),
                    Logo = Utils.UploadFile(model.Logo, model.ExistingLogo, _hostingEnvironment, "logos"),
                    EmployerId = _userManager.GetUserId(User)
                };

                var result = await _companyRepo.AddCompany(company);
                if (!result)
                {
                    _logger.LogError($"Error saving to database!");
                    ViewBag.ErrorMessage = "Failed to save to database!";
                }
                return RedirectToAction("Company");
            };
            return View(model);
        }
        //--------------------------------------------------------------------------------------------------------

        // Delete company (POST)
        [HttpPost]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _companyRepo.GetCompany(id);
            if(company != null)
            {
                var result = await _companyRepo.DeleteCompany(company);
                if (result)
                {
                    if(company.Logo != null)
                    {
                        Utils.DeleteFile(company.Logo, _hostingEnvironment, "logos");
                    }
                    if (company.CompanyCertificate != null)
                    {
                        Utils.DeleteFile(company.CompanyCertificate, _hostingEnvironment, "Certificates");
                    }
                    return RedirectToAction("Company");
                }
                _logger.LogError($"Error deleting company!");
                ViewBag.ErrorMessage = "Failed to delete company!";
            }
            return View();
        }
    }
}
