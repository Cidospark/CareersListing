using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareersListing.Models;
using CareersListing.Utilities;
using CareersListing.ViewModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CareersListing.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVacancyRepo _vacancyRepo;
        private readonly ICompanyRepo _companyRepo;

        public HomeController(IVacancyRepo vacancyRepo, ICompanyRepo companyRepo)
        {
            _vacancyRepo = vacancyRepo;
            _companyRepo = companyRepo;
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
    }
}
