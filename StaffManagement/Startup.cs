using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using StaffManagement.Common;
using StaffManagement.Data;
using StaffManagement.Models;
using StaffManagement.Repositories;
using StaffManagement.Repositories.Interfaces;
using StaffManagement.Services;
using static StaffManagement.Models.CustomAttributes.ValidIfLongTermSub;

namespace StaffManagement
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        /// <summary>
        /// Add desired services to the services collection. 
        /// Also add custom classes to the services collection to allow them to be injected into other classes (See AccountController.cs)
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddDistributedMemoryCache();

            services.AddDataProtection().SetApplicationName("URL");

            services.AddSession();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddAuthorization();

            services.AddAuthentication(IISDefaults.AuthenticationScheme);

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.AppendTrailingSlash = true;
            });

            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

            services.AddDbContext<accountmanagementContext>
                (options => options
                .UseSqlServer(Configuration.GetConnectionString(Configuration.GetConnectionString("ActiveDB")))
                .EnableSensitiveDataLogging());


            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });


            services.AddSingleton<IEmailSettings>(Configuration.GetSection("EmailSettings").Get<EmailSettings>());

            services.AddControllers();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<SiteText, SiteText>();
            services.AddSingleton<URLEncrypter, URLEncrypter>();
            services.AddSingleton<IValidationAttributeAdapterProvider, ValidIfLongTermSubAdapterProvider>();


            services.AddScoped<IEmail, Email>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IJobRecordRepository, JobRecordRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<ISystemReviewRepository, SystemReviewRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IAuditLoggerService, AuditLoggerService>();
            services.AddScoped<AppUser, AppUser>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                var options = new DeveloperExceptionPageOptions
                {   //Show 10 lines on debug page
                    SourceCodeLineCount = 10
                };
                app.UseDeveloperExceptionPage(options);
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseCookiePolicy();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}/{slug?}");
            });


        }


    }
}
