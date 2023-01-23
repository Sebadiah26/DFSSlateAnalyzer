using DFSSlateAnalyzerCore.Repositories.Interfaces;
using DFSSlateAnalyzerCore.Repositories;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using DFSSlateAnalyzerData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var services = builder.Services;

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();


services.AddDistributedMemoryCache();
services.AddHttpContextAccessor();


services.AddScoped<ISlateRepository, SlateRepository>();

services.AddDbContext<DFSSlateAnalyzerContext>
              (options => options
              .UseSqlServer(Configuration.GetConnectionString(Configuration.GetConnectionString("ActiveDB")))
              .EnableSensitiveDataLogging());

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
