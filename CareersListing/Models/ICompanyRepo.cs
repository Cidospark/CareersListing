using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Models
{
    public interface ICompanyRepo
    {
        Task<Company> GetCompany(int? Id);
        Task<ICollection<Company>> GetAllCompanies();
        Task<ICollection<Company>> GetAllCompaniesByEmployer(string EmployerId);
        Task<bool> AddCompany(Company company);
        Task<bool> UpdateCompany(Company company);
        Task<bool> DeleteCompany(Company company);
        Task<bool> Save();
    }
}
