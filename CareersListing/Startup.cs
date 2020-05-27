using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareersListing.Models;
using CareersListing.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CareersListing
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("default")));
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 3;

                // settings for email lockout count and lock time
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 3;

                // setting for confirmed email on sign in and customized email token life span
                options.SignIn.RequireConfirmedEmail = true; // step 1
                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");
            // Customize the lifespan of email token generated in the app to 3 days instead of the default 5 hours
            services.Configure<CustomEmailConfirmationTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromDays(3));

            services.AddAuthorization(options =>  {
                options.AddPolicy("AdminRole", policy => policy.RequireAssertion(context => context.User.IsInRole("Super Admin") 
                                                        || context.User.IsInRole("Admin"))
                );

                options.AddPolicy("EmployerRole", policy => policy.RequireRole("Employer"));
                options.AddPolicy("Applicant", policy => policy.RequireRole("Applicant"));

            });

            // set all lifespan of all tokens generated in the app to 5 hours
            services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(5));

            services.AddScoped<ICompanyRepo, CompanyRepo>();
            services.AddScoped<IVacancyRepo, VacancyRepo>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
                    ApplicationDbContext ctxt, UserManager<ApplicationUser> userManager, 
                    RoleManager<IdentityRole> roleManager, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes => {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            Seed.SeedUsers(ctxt, userManager, roleManager, logger).Wait();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
