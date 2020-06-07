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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CareersListing.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVacancyRepo _vacancyRepo;
        private readonly ICompanyRepo _companyRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJobApplicationRepo _jobApplicationRepo;

        public HomeController(IVacancyRepo vacancyRepo, 
            ICompanyRepo companyRepo, 
            UserManager<ApplicationUser> userManager,
            IJobApplicationRepo jobApplicationRepo)
        {
            _vacancyRepo = vacancyRepo;
            _companyRepo = companyRepo;
            _userManager = userManager;
            _jobApplicationRepo = jobApplicationRepo;
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
        public async Task<IActionResult> Job(int id, int? status)
        {
            var job = await _vacancyRepo.GetVacancy(id);
            if (job == null)
            {
                ViewBag.ErrorMessage = $"User with id : {id} was not found!";
                return View("NotFound");
            }
            var currentUserId = _userManager.GetUserId(User);
            var isAppliedFor = await _jobApplicationRepo.ApplicationExists(currentUserId, id);

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
                DateExpired = job.DateExpired.ToLongDateString(),
                ApplicationExists = isAppliedFor
            };

            var company = await _companyRepo.GetCompany(job.CompanyId);
            if(company != null)
                model.Company = company;

            var employer = await _userManager.FindByIdAsync(job.EmployerId);
            if(employer != null)
                model.Employer = employer;

            if(status == 1)
            {
                ViewBag.ResultMessage = "Congratutions! You have applied for this job.";
            }else if (status == -1)
            {
                ViewBag.ResultMessage = "Sorry! Your application failed. Please try again.";
            }
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


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Apply(int id)
        {
            // exit if job has been applied for by current user?
            var currentUserId = _userManager.GetUserId(User);
            var isAppliedFor = await _jobApplicationRepo.ApplicationExists(currentUserId, id);
            if (isAppliedFor)
                return RedirectToAction("Job", new { id });

            // add application record to database
            var model = new JobApplication
            {
                ApplicantId = currentUserId,
                VacancyId = id,
                DateRegistered = DateTime.Now
            };
            var result = await _jobApplicationRepo.AddApplication(model);
            if (!result)
            {
                return RedirectToAction("Job", new { id, status = -1 });
            }

            return RedirectToAction("Job", new { id, status = 1});
        }
    }
}
