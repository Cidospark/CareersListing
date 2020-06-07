using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareersListing.Models;
using CareersListing.Utilities;
using CareersListing.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CareersListing.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVacancyRepo _vacancyRepo;
        private readonly ICompanyRepo _companyRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IVacancyRepo vacancyRepo, ICompanyRepo companyRepo, UserManager<ApplicationUser> userManager)
        {
            _vacancyRepo = vacancyRepo;
            _companyRepo = companyRepo;
            _userManager = userManager;
        }

        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // get a list of vacancies
            var listings = await _vacancyRepo.GetAllRecentVacancies();
            var model = new HomeViewModel();
            if(listings == null)
            {
                return View();
            }
            
            foreach(var row in listings)
            {

                var listing = new ListingViewModel
                {
                    Id = row.Id,
                    CompanyId = row.CompanyId,
                    JobTitle = row.JobTitle,
                    DaysAgo = Utils.GetDayAgo(row.DateExpired),
                    Industry = row.Industry,
                    Duration = row.JobDuration,
                    Location = row.Location
                };

                var company = await _companyRepo.GetCompany(row.CompanyId);
                listing.Company = company;

                model.Listings.Add(listing);
            }
            model.NumberOfListings = listings.Count();
            model.NumberOfRegisteredCompanies = _companyRepo.GetAllCompanies().Result.Count();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Jobs()
        {
            // get a list of vacancies
            var listings = await _vacancyRepo.GetAllVacancies();
            var model = new HomeViewModel();
            if (listings == null)
            {
                return View();
            }

            foreach (var row in listings)
            {

                var listing = new ListingViewModel
                {
                    Id = row.Id,
                    CompanyId = row.CompanyId,
                    JobTitle = row.JobTitle,
                    DaysAgo = Utils.GetDayAgo(row.DateExpired),
                    Industry = row.Industry,
                    Duration = row.JobDuration,
                    Location = row.Location
                };

                var company = await _companyRepo.GetCompany(row.CompanyId);
                if (company != null)
                    listing.Company = company;

                model.Listings.Add(listing);
            }
            model.NumberOfListings = listings.Count();
            model.NumberOfRegisteredCompanies = _companyRepo.GetAllCompanies().Result.Count();
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Job(int id)
        {
            var job = await _vacancyRepo.GetVacancy(id);
            if (job == null)
            {
                ViewBag.ErrorMessage = $"User with id : {id} was not found!";
                return View("NotFound");
            }
            
            var model = new JobViewModel
            {
                Id = job.Id,
                CompanyId = job.CompanyId,
                EmployerId = job.EmployerId,
                JobTitle = job.JobTitle,
                JobFunction = job.JobFunction,
                Industry = job.Industry,
                Location = job.Location,
                Description = job.Description,
                SalaryScale = job.SalaryScale,
                DaysAgo = Utils.GetDayAgo(job.DateExpired),
                JobDuration = job.JobDuration,
                DateExpired = job.DateExpired.ToLongDateString()
            };

            var company = await _companyRepo.GetCompany(job.CompanyId);
            if(company != null)
                model.Company = company;

            var employer = await _userManager.FindByIdAsync(job.EmployerId);
            if(employer != null)
                model.Employer = employer;

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Search(HomeViewModel model)
        {
            var searchResult = await _vacancyRepo.SearchVacancy(model.JobFunction, model.Industry, model.Location);
            if(searchResult == null)
            {
                model.NumberOfListings = 0;
                return View(model);
            }

            foreach (var row in searchResult)
            {

                var listing = new ListingViewModel
                {
                    Id = row.Id,
                    CompanyId = row.CompanyId,
                    JobTitle = row.JobTitle,
                    DaysAgo = Utils.GetDayAgo(row.DateExpired),
                    Industry = row.Industry,
                    Duration = row.JobDuration,
                    Location = row.Location
                };

                var company = await _companyRepo.GetCompany(row.CompanyId);
                listing.Company = company;

                model.Listings.Add(listing);
            }
            model.NumberOfListings = searchResult.Count();
            model.NumberOfRegisteredCompanies = _companyRepo.GetAllCompanies().Result.Count();
            return View(model);
        }

    }
}
