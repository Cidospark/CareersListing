using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Models
{
    public class VacancyRepo : IVacancyRepo
    {
        private readonly ApplicationDbContext _context;

        public VacancyRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task<ICollection<Vacancy>> GetAllVacanciesByEmployer(string EmployerId)
        {
            return await _context.Vacancies.Where(v => v.EmployerId == EmployerId).OrderByDescending(v => v.Id).ToListAsync();
        }

        public async Task<ICollection<Vacancy>> GetAllVacancies()
        {
            return await _context.Vacancies.OrderByDescending(v => v.Id).ToListAsync();
        }

        public async Task<ICollection<Vacancy>> GetAllRecentVacancies()
        {
            return await _context.Vacancies.Where(v => v.DateExpired > DateTime.Now).OrderByDescending(v => v.Id).ToListAsync();
        }

        public async Task<Vacancy> GetVacancy(int? Id)
        {
            return await _context.Vacancies.FindAsync(Id);
        }

        // commit changes to database
        public async Task<bool> Save()
        {
            System.Int32 saved = await _context.SaveChangesAsync();
            return saved >= 0 ? true : false;
        }

        public async Task<bool> AddVacancy(Vacancy vacancy)
        {
            await _context.AddAsync(vacancy);
            return await Save();
        }

        public async Task<bool> DeleteVacancy(Vacancy vacancy)
        {
            _context.Remove(vacancy);
            return await Save();
        }

        public async Task<bool> UpdateVacancy(Vacancy vacancy)
        {
            var entry = _context.Vacancies.Attach(vacancy);
            entry.State = EntityState.Modified;
            return await Save();
        }
    }
}
