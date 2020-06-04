using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Models
{
    public interface IVacancyRepo
    {
        Task<Vacancy> GetVacancy(int? Id);
        Task<ICollection<Vacancy>> GetAllVacancies();
        Task<ICollection<Vacancy>> GetAllRecentVacancies();
        Task<ICollection<Vacancy>> GetAllVacanciesByEmployer(string EmployerId);
        Task<bool> AddVacancy(Vacancy vacancy);
        Task<bool> UpdateVacancy(Vacancy vacancy);
        Task<bool> DeleteVacancy(Vacancy vacancy);
        Task<bool> Save();
    }
}
