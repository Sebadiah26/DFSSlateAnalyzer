using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StaffManagement.Core.Common;
using StaffManagement.Core.Repositories;
using StaffManagement.Core.Repositories.Interfaces;
using StaffManagement.Core.Services;
using StaffManagement.Data;

namespace StaffManagement.Core.Extensions
{
    /// <summary>
    /// Single entry point for wiring up everything StaffManagement.Core owns: the DbContext and
    /// every scoped service/repository. Program.cs calls this once and never registers
    /// AddDbContext itself, matching the DFSSlateAnalyzerCoreServiceExtensions convention.
    /// ICurrentUserContext itself is NOT registered here - it's implemented by
    /// StaffManagement.Blazor.Services.CurrentUserService and registered in Program.cs, since it's a
    /// circuit-scoped concept that belongs to the Blazor layer, not Core.
    /// </summary>
    public static class StaffManagementCoreServiceExtensions
    {
        /// <summary>
        /// Takes the host's own IConfiguration (WebApplicationBuilder.Configuration) rather than
        /// building a separate one from just appsettings.json - the reference DFSSlateAnalyzerCore
        /// convention this was originally ported from did the latter, which silently skips
        /// appsettings.{Environment}.json (and env vars/user secrets). That matters here: test-mode's
        /// local-database override lives in appsettings.Development.json's ConnectionStrings:ActiveDB,
        /// and it would never be picked up without environment layering.
        /// </summary>
        public static IServiceCollection AddStaffManagementCoreClasses(this IServiceCollection services, IConfiguration config)
        {
            // Same double-indirection idiom as the legacy app: ConnectionStrings:ActiveDB names
            // which other ConnectionStrings key to actually use (e.g. "AccountManagementDBLive").
            //
            // AddDbContextFactory (not AddDbContext) deliberately: in Blazor Server, sibling
            // components on the same page (e.g. ImpersonateWidget + EmployeeFilter, both in
            // MainLayout) run their OnInitializedAsync concurrently, and a single scoped DbContext
            // instance shared across them throws "A second operation was started on this context
            // instance before a previous operation completed." Every repository/service here creates
            // a short-lived context per operation via the factory instead of holding one shared
            // instance - see BaseRepository/UserRepository/UnitRepository/AuditLoggerService.
            services.AddDbContextFactory<accountmanagementContext>(options => options
                .EnableSensitiveDataLogging()
                .UseSqlServer(config.GetConnectionString(config.GetConnectionString("ActiveDB") ?? "")));

            services.AddMemoryCache();

            services.AddScoped<IAuditLoggerService, AuditLoggerService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IContractRepository, ContractRepository>();
            services.AddScoped<ISystemReviewRepository, SystemReviewRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IJobRecordRepository, JobRecordRepository>();

            services.AddSingleton<URLEncrypter>();
            services.AddSingleton<IEmailSettings>(config.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings());
            services.AddScoped<IEmail, Email>();

            return services;
        }
    }
}
