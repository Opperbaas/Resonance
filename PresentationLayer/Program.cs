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
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Authentication and hashing
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, SimplePasswordHasher>();
builder.Services.AddScoped<IEmailSender, ConsoleEmailSender>();

// song management for library
builder.Services.AddScoped<ISongService, SongService>();

// session support for MVC views
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

// Allow both API controllers and MVC views
builder.Services.AddControllersWithViews();

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
            Email = "admin@resonance.local",
            PasswordHash = hasher.Hash("password"),
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();
    }
}

// static files still ok if you have swagger, assets, etc.
app.UseStaticFiles();

// session middleware must be before routing
app.UseSession();

// friendly MVC routes (login/register) kept from earlier example
app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "AuthMvc", action = "Login" });
app.MapControllerRoute(
    name: "register",
    pattern: "register",
    defaults: new { controller = "AuthMvc", action = "Register" });

// Map traditional MVC routes (for view‑rendering controllers)
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

app.Run();
