using System;
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
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISpecificService, SpecificService>();

// Authentication and hashing
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, SimplePasswordHasher>();

// MVC + Views
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var app = builder.Build();

// Seed a default user for demo purposes (use migrations and secure seeding in production)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    db.Database.EnsureCreated();
    if (!db.Users.Any())
    {
        db.Users.Add(new Resonance.DataAccessLayer.Models.User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            PasswordHash = hasher.Hash("password"),
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();
    }
}

app.UseStaticFiles();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

app.Run();
