using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareersListing.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options): base(options) {}

        public DbSet<Company> Companies { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // the line below have to be called 
        //    // since we are overriding OnModelCreating
        //    base.OnModelCreating(modelBuilder);

        //    //foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        //    //{
        //    //    foreignKey.DeleteBehavior = DeleteBehavior.Cascade;
        //    //}
        //}
    }
}
