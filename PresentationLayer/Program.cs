using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Resonance.DataAccessLayer.Context;
using Resonance.DataAccessLayer.Interfaces;
using Resonance.DataAccessLayer.Models;
using Resonance.DataAccessLayer.Repositories;
using Resonance.DataAccessLayer.UnitOfWork;
using Resonance.BusinessLogicLayer.Integrations;
using Resonance.BusinessLogicLayer.Interfaces;
using Resonance.BusinessLogicLayer.Services;
var builder = WebApplication.CreateBuilder(args);

// Configure DbContext (replace with your actual connection string)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? ""));


// Register repositories, unit of work and services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Authentication and hashing
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, SimplePasswordHasher>();
builder.Services.AddScoped<IEmailSender, ConsoleEmailSender>();

// profile services
builder.Services.AddScoped<IProfileService, ProfileService>();

// track management for library
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.Configure<SpotifyApiSettings>(builder.Configuration.GetSection("SpotifyApi"));
builder.Services.AddHttpClient<SpotifyApiClient>(client => {
    client.BaseAddress = new Uri("https://api.spotify.com/v1/");
});
builder.Services.AddScoped<IMusicImportProvider, SpotifyImportProvider>();
builder.Services.AddScoped<IMusicImportService, MusicImportService>();
builder.Services.AddScoped<IAudioFeatureService, AudioFeatureService>();

// mood/play tracking services
builder.Services.AddScoped<IMoodEntryService, MoodEntryService>();
builder.Services.AddScoped<IPlayEventService, PlayEventService>();
builder.Services.AddScoped<IMoodEntryPlayLinkService, MoodEntryPlayLinkService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

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

    if (!db.MoodTypes.Any())
    {
        db.MoodTypes.AddRange(
            new MoodType { Label = "Happy", Emoji = "😊", ColorHex = "#F6C343", IsActive = true },
            new MoodType { Label = "Sad", Emoji = "😢", ColorHex = "#4A90E2", IsActive = true },
            new MoodType { Label = "Energetic", Emoji = "⚡", ColorHex = "#E94B3C", IsActive = true },
            new MoodType { Label = "Calm", Emoji = "😌", ColorHex = "#7ED321", IsActive = true }
        );
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
