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

var builder = WebApplication.CreateBuilder(args);


IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Add services to the container.

var services = builder.Services;


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

app.Run();
