using DFSSlateAnalyzerCore.Repositories.Interfaces;
using DFSSlateAnalyzerCore.Repositories;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using DFSSlateAnalyzerData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using DFSSlateAnalyzerCore.Extensions;
using Microsoft.Extensions.FileProviders;

using static DFSSlateAnalyzerCore.Services.FileServerProviderService;
using DFSSlateAnalyzerCore.Services;
//using static DFSSlateAnalyzerAPI.Services.FileServerProviderService;

var builder = WebApplication.CreateBuilder(args);
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Add services to the container.

var services = builder.Services;

//Add our IFileServerProvider implementation as a singleton
//builder.Services.AddSingleton<DFSSlateAnalyzerAPI.Services.FileServerProviderService.IFileServerProvider>(new DFSSlateAnalyzerAPI.Services.FileServerProviderService.FileServerProvider(
//    new List<FileServerOptions>
//    {
//            new FileServerOptions
//            {
//                FileProvider = new PhysicalFileProvider(@"\\DESKTOP-FT0FCJQ\DFSAnalyzer"),
//                RequestPath = new PathString("/files"),
//                EnableDirectoryBrowsing = true
//            },
//            //new FileServerOptions
//            //{
//            //    FileProvider = new PhysicalFileProvider(@"\\server\path"),
//            //    RequestPath = new PathString("/MyPath"),
//            //    EnableDirectoryBrowsing = true
//            //}
//    }));


services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();


services.AddDistributedMemoryCache();
services.AddHttpContextAccessor();


builder.Services.AddMvc()
                .AddApplicationPart(typeof(ISlateRepository).Assembly)
              
                ;

             

builder.Services.AddDFSSlateAnalyzerCoreClasses();


IFileProvider physicalProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
//"C:\\Users\\sebad\\source\\repos\\Sebadiah26\\DFSSlateAnalyzer\\DFSSlateAnalyzerAngular"


builder.Services.AddSingleton<IFileProvider>(physicalProvider);


//services.AddTransient<ISlateRepository, SlateRepository>();

//services.AddDbContext<DFSSlateAnalyzerContext>();
//(options => options
//.UseSqlServer(config.GetConnectionString(config.GetConnectionString("ActiveDB") ?? ""))
// .EnableSensitiveDataLogging());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "MyStaticFiles")),
    RequestPath = "/StaticFiles"
});

// C:\Users\sebad\source\repos\Sebadiah26\DFSSlateAnalyzer\DFSSlateAnalyzer


app.UseFileServerProvider(app.Services.GetService<IFileServerProvider>());

app.Run();
