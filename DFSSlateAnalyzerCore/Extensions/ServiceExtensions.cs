
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DFSSlateAnalyzerCore.Repositories.Interfaces;
using DFSSlateAnalyzerCore.Repositories;
using DFSSlateAnalyzerData;
using Microsoft.EntityFrameworkCore;


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

                return services;
            }
        }

   
}
