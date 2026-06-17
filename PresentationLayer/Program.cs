using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5001, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

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
builder.Services.Configure<YouTubeApiSettings>(builder.Configuration.GetSection("YouTubeApi"));
builder.Services.AddHttpClient<YouTubeApiClient>(client => {
    client.BaseAddress = new Uri("https://www.googleapis.com/youtube/v3/");
});
builder.Services.AddScoped<IMusicImportProvider, SpotifyImportProvider>();
builder.Services.AddScoped<IMusicImportProvider, YouTubeImportProvider>();
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
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.IsEssential = true;
});

builder.Services.AddCookiePolicy(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
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


app.UseHttpsRedirection();
app.UseCookiePolicy();

app.Use(async (context, next) =>
{
    if (context.Request.Host.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
    {
        var builder = new UriBuilder
        {
            Scheme = context.Request.Scheme,
            Host = "127.0.0.1",
            Port = context.Request.Host.Port ?? (context.Request.IsHttps ? 443 : 80),
            Path = context.Request.Path.HasValue ? context.Request.Path.Value : string.Empty,
            Query = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty
        };

        context.Response.Redirect(builder.ToString(), permanent: false);
        return;
    }

    await next();
});

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

// Map Spotify callback routes explicitly
app.MapControllerRoute(name: "spotify_callback", pattern: "callback", defaults: new { controller = "Spotify", action = "Callback" });
app.MapControllerRoute(name: "spotify_callback_alt", pattern: "spotify/callback", defaults: new { controller = "Spotify", action = "Callback" });

// Map traditional MVC routes (for view‑rendering controllers)
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

app.Run();
