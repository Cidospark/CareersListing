using CareersListing.Models;
using CareersListing.Utilities;
using CareersListing.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Employer, Super Admin, Admin")]
    public class EmployerController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ICompanyRepo _companyRepo;
        private readonly ILogger<EmployerController> _logger;
        private readonly IVacancyRepo _vacancyRepo;

        public EmployerController(RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager,
                                        IHostingEnvironment hostingEnvironment,
                                        ICompanyRepo companyRepo, ILogger<EmployerController> logger,
                                        IVacancyRepo vacancyRepo)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _companyRepo = companyRepo;
            _logger = logger;
            _vacancyRepo = vacancyRepo;
        }
        // ----------------------------------------------------------------------------------------

        // Vacancies (GET)
        [HttpGet]
        public async Task<IActionResult> Vacancies()
        {
            JobVacancyViewModel model = new JobVacancyViewModel();
            List<ListCompaniesViewModel> listOfCompanies = new List<ListCompaniesViewModel>();
            List<ListOfJobVacancies> listOfVacancies = new List<ListOfJobVacancies>();
            
            var user = await _userManager.GetUserAsync(User);
            ICollection<Company> companies = null;
            ICollection<Vacancy> vacancies = null;
            if (await _userManager.IsInRoleAsync(user, "Super Admin") || await _userManager.IsInRoleAsync(user, "Admin"))
            {
                companies = await _companyRepo.GetAllCompanies();
                vacancies = await _vacancyRepo.GetAllVacancies();
            }
            else 
            { 
                companies = await _companyRepo.GetAllCompaniesByEmployer(_userManager.GetUserId(User));
                vacancies = await _vacancyRepo.GetAllVacanciesByEmployer(_userManager.GetUserId(User));
            }
            

            // add the list of companies to ViewBag.CompanyList
            foreach (var company in companies)
            {
                var row = new ListCompaniesViewModel
                {
                    Id = company.Id,
                    Name = company.Name,
                };

                listOfCompanies.Add(row);
            }

            // add list of job vacancies to model
            foreach(var vacancy in vacancies)
            {
                var row = new ListOfJobVacancies
                {
                    Id = vacancy.Id,
                    JobTitle = vacancy.JobTitle,
                    Company = vacancy.Company,
                    Location = vacancy.Location,
                    Salaries = vacancy.SalaryScale,
                    Duration = vacancy.JobDuration,
                    DateExpired = vacancy.DateExpired
                };
                model.Vacancies.Add(row);
            }

            ViewBag.CompanyList = listOfCompanies;
            return View(model);
        }
        // Company (POST) ---------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Vacancies(JobVacancyViewModel model, int? id)
        {
            if (ModelState.IsValid)
            {
                var vacancy = new Vacancy
                {
                   CompanyId = model.CompanyId,
                   EmployerId = _userManager.GetUserId(User),
                   JobTitle = model.JobTitle,
                   JobDuration = model.JobDuration,
                   JobFunction = model.JobFunction,
                   Industry = model.Industry,
                   Location = model.Location,
                   SalaryScale = model.SalaryScale,
                   DateExpired = model.DateExpired,
                   DatePosted = model.DatePosted,
                   Description = model.Description
                };

                // id value is binding from the edit view 
                bool result;
                if (id.HasValue)
                {
                    vacancy.Id = (int)id;
                    result = await _vacancyRepo.UpdateVacancy(vacancy);
                }
                else
                {
                    result = await _vacancyRepo.AddVacancy(vacancy);
                }

                if (!result)
                {
                    _logger.LogError($"Error saving vacancy to database!");
                    ViewBag.ErrorMessage = "Failed to save vacancy to database!";
                }
                return RedirectToAction("Vacancies");
            };
            return View(model);
        }
        //--------------------------------------------------------------------------------------------------------

        // Delete company (POST)
        [HttpPost]
        public async Task<IActionResult> DeleteVacancy(int id)
        {
            var vacancy = await _vacancyRepo.GetVacancy(id);
            if (vacancy != null)
            {
                var result = await _vacancyRepo.DeleteVacancy(vacancy);
                if (result)
                {
                    return RedirectToAction("Vacancy");
                }
                _logger.LogError($"Error deleting vacancy!");
                ViewBag.ErrorMessage = "Failed to delete vacancy!";
            }
            return View();
        }
        //--------------------------------------------------------------------------------------------------------

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
            var user = await _userManager.GetUserAsync(User);
            ICollection<Company> companies = null;
            if (await _userManager.IsInRoleAsync(user, "Super Admin") || await _userManager.IsInRoleAsync(user, "Admin"))
            {
               companies = await _companyRepo.GetAllCompanies();
            }
            else
            {
                companies = await _companyRepo.GetAllCompaniesByEmployer(_userManager.GetUserId(User));
            }
            
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
                    _logger.LogError($"Error saving company to database!");
                    ViewBag.ErrorMessage = "Failed to save company to database!";
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
