using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Resonance.DataAccessLayer.Context;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Repositories;
using Resonance.DataAccessLayer.UnitOfWork;
using Resonance.BusinessLogicLayer.Interfaces;
using Resonance.BusinessLogicLayer.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext (replace with your actual connection string)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? ""));

// Register repositories, unit of work and services
builder.Services.AddScoped<ISpecificRepository, SpecificRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISpecificService, SpecificService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
