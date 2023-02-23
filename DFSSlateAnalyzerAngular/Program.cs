using DFSSlateAnalyzerAngular.Services;
using DFSSlateAnalyzerCore.Extensions;
using DFSSlateAnalyzerCore.Repositories;
using DFSSlateAnalyzerCore.Repositories.Interfaces;
using Microsoft.Extensions.FileProviders;
using System.Data.Entity;
using System.Reflection;
//using static DFSSlateAnalyzerAngular.Services.FileServerProviderService;
//using static DFSSlateAnalyzerAPI.Services.FileServerProviderService;
using DFSSlateAnalyzerAPI.Services;
using static DFSSlateAnalyzerCore.Services.FileServerProviderService;
using DFSSlateAnalyzerCore.Services;

var builder = WebApplication.CreateBuilder(args);


////Add our IFileServerProvider implementation as a singleton
//builder.Services.AddSingleton<DFSSlateAnalyzerAngular.Services.FileServerProviderService.IFileServerProvider>(new DFSSlateAnalyzerAngular.Services.FileServerProviderService.FileServerProvider(
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

// Add services to the container.

builder.Services.AddControllersWithViews();



//var controllerAssembly = Assembly.Load(new AssemblyName("DFSSlateAnalyzerAPI"));


builder.Services.AddMvc()
                .AddApplicationPart(typeof(ISlateRepository).Assembly)
              //  .AddApplicationPart(controllerAssembly)
            //    .AddControllersAsServices()
                ;


builder.Services.AddDFSSlateAnalyzerCoreClasses();



IFileProvider physicalProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
//"C:\\Users\\sebad\\source\\repos\\Sebadiah26\\DFSSlateAnalyzer\\DFSSlateAnalyzerAngular"


builder.Services.AddSingleton<IFileProvider>(physicalProvider);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


//builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
//app.UseMvcWithDefaultRoute();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

//endpoints.MapControllerRoute(
//                    name: "default",
//                    pattern: "{controller}/{action}/{id?}");

app.MapFallbackToFile("index.html");

//app.UseFileServer(new FileServerOptions
//{
//    FileProvider = new PhysicalFileProvider(@"\\DESKTOP-FT0FCJQ\DFSAnalyzer"),
//    RequestPath = new PathString("/files"),
//    EnableDirectoryBrowsing = true
//});

//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//           Path.Combine(builder.Environment.ContentRootPath, "MyStaticFiles")),
//    RequestPath = "/StaticFiles"
//});44

//"C:\\Users\\sebad\\source\\repos\\Sebadiah26\\DFSSlateAnalyzer\\DFSSlateAnalyzerAngular\\"   --content root
// "C:\\Users\\sebad\\source\\repos\\Sebadiah26\\DFSSlateAnalyzer\\DFSSlateAnalyzerAngular\\wwwroot"  --web root path


app.UseFileServerProvider(app.Services.GetService<IFileServerProvider>());

app.Run();
 