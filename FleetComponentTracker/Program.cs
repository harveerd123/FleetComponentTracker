using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using FleetComponentTracker.Components;
using FleetComponentTracker.Database;
using FleetComponentTracker.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Register the ApplicationDbContext with dependency injection, using SQLite as the database provider
builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlite("Data Source=fleettracker.db"));

// Register Blazorise UI framework with Bootstrap 5 styling and FontAwesome icons
builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

// Add developer exception filter for database-related errors during development
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register scoped services for dependency injection
builder.Services.AddScoped<VehicleDataService>();
builder.Services.AddScoped<CsvImportService>();

// Add Controllers (for API endpoints)
builder.Services.AddControllers();

// Register an HttpClient scoped service with the base address set to the current app base URI
// This allows making HTTP requests back to this app (e.g., calling API controllers)
builder.Services.AddScoped(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMigrationsEndPoint();
}

// Seed initial data into the database on app startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();

    var dataService = scope.ServiceProvider.GetRequiredService<VehicleDataService>();
    dataService.SeedVehicleData();
}

    app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();
