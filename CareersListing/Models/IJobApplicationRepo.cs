using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Models
{
    public interface IJobApplicationRepo
    {
        Task<bool> AddApplication(JobApplication jobApplication);
        Task<bool> ApplicationExists(string userId, int vacancyId);
        Task<List<JobApplication>> GetApplications();
        //Task<JobApplication> GetApplication(int Id);
        //Task<JobApplication> GetApplication(string Id);
    }
}
