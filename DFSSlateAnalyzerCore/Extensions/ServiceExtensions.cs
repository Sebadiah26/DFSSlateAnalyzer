using DFSSlateAnalyzerCore.Repositories;
using DFSSlateAnalyzerCore.Repositories.Interfaces;
using DFSSlateAnalyzerData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using static DFSSlateAnalyzerCore.Services.FileServerProviderService;

namespace DFSSlateAnalyzerCore.Extensions
{


    public static class DFSSlateAnalyzerCoreServiceExtensions
    {
        public static IServiceCollection AddDFSSlateAnalyzerCoreClasses(this IServiceCollection services)
        {


            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();


            services.AddDbContext<DFSSlateAnalyzerContext>(options => options.EnableSensitiveDataLogging().UseSqlServer(config.GetConnectionString(config.GetConnectionString("ActiveDB") ?? ""), b => b.MigrationsAssembly("DFSSlateAnalyzerAPI"))
 );
            services.AddScoped<ISlateRepository, SlateRepository>();

            //Add our IFileServerProvider implementation as a singleton
            services.AddSingleton<IFileServerProvider>(new FileServerProvider(
                    new List<FileServerOptions>
                    {
                        new FileServerOptions
                        {
                            //FileProvider = new PhysicalFileProvider(@"\\DESKTOP-FT0FCJQ\DFSAnalyzer"),
                            FileProvider = new PhysicalFileProvider(config.GetSection("FilePath").Value ?? "" ),
                            RequestPath = new PathString("/files"),
                            EnableDirectoryBrowsing = true
                        },

                    }));


            return services;
        }
    }


}
