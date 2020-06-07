using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Models
{
    public class JobApplicationRepo : IJobApplicationRepo
    {
        private readonly ApplicationDbContext _context;

        public JobApplicationRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Saved()
        {
            var saved = await _context.SaveChangesAsync();
            return saved >= 0 ? true : false;
        }

        public async Task<bool> AddApplication(JobApplication jobApplication)
        {
            await _context.AddAsync(jobApplication);
            return await Saved();
        }

        public async Task<List<JobApplication>> GetApplications()
        {
            return await _context.JobApplications.OrderByDescending(j => j.Id).ToListAsync();
        }

        public async Task<bool> ApplicationExists(string userId, int vacancyId)
        {
            return await _context.JobApplications.AnyAsync(j => j.ApplicantId == userId && j.VacancyId == vacancyId);
        }
    }
}
