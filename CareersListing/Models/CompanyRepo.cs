using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Models
{
    public class CompanyRepo : ICompanyRepo
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // get all companies
        public async Task<ICollection<Company>> GetAllCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        // filter companies by employer
        public async Task<ICollection<Company>> GetAllCompaniesByEmployer(string EmployerId)
        {
            return await _context.Companies.Where(e => e.EmployerId == EmployerId).ToListAsync();
        }

        // get single company
        public async Task<Company> GetCompany(int? Id)
        {
            return await _context.Companies.FindAsync(Id);
        }

        // commit changes to database
        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved >= 0 ? true : false;
        }

        // add company
        public async Task<bool> AddCompany(Company company)
        {
            await _context.AddAsync(company);
            return await Save();
        }

        // delete company
        public async Task<bool> DeleteCompany(Company company)
        {
            _context.Remove(company);
            return await Save();
        }

        // update company
        public async Task<bool> UpdateCompany(Company company)
        {
            var entry = _context.Companies.Attach(company);
            entry.State = EntityState.Modified;
            return await Save();
        }
    }
}
