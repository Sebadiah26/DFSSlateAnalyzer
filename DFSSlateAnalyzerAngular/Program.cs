using DFSSlateAnalyzerCore.Extensions;
using DFSSlateAnalyzerCore.Repositories;
using DFSSlateAnalyzerCore.Repositories.Interfaces;
using System.Data.Entity;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();



//var controllerAssembly = Assembly.Load(new AssemblyName("DFSSlateAnalyzerAPI"));
//builder.Services.AddMvc().AddApplicationPart(controllerAssembly).AddControllersAsServices();

builder.Services.AddMvc()
                .AddApplicationPart(typeof(ISlateRepository).Assembly)
                ;


builder.Services.AddDFSSlateAnalyzerCoreClasses();


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

app.MapFallbackToFile("index.html");

app.Run();
