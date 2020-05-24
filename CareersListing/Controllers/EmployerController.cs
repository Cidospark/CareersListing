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
        public async Task<IActionResult> Company(int? id)
        {
            CompanyViewModel model = new CompanyViewModel();
            if (id.HasValue)
            {
                var companyToEdit = await _companyRepo.GetCompany(id);
                if (companyToEdit == null)
                {
                    ViewBag.ErrorMessage = $"Company with id : {id} was not found!";
                    return RedirectToAction("NotFound");
                }
                    // add the details of company to edit to model
                    model.Id = companyToEdit.Id;
                    model.Name = companyToEdit.Name;
                    model.Email = companyToEdit.Email;
                    model.Website = companyToEdit.Website;
                    model.Street = companyToEdit.Street;
                    model.City = companyToEdit.Website;
                    model.Country = companyToEdit.Country;
                    model.ExistingLogo = companyToEdit.Logo;
                    model.ExistingCertificate = companyToEdit.CompanyCertificate;
            }

            // add the list of companies to model
            var companies = await _companyRepo.GetAllCompanies();
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
        public async Task<IActionResult> Company(CompanyViewModel model, int? id)
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

                // id value is binding from the edit view 
                bool result;
                if (id.HasValue)
                {
                    company.Id = (int)id;
                    result = await _companyRepo.UpdateCompany(company);
                }
                else
                {
                    result = await _companyRepo.AddCompany(company);
                }
                
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
