using AQI_Monitoring_System.Data;
using AQI_Monitoring_System.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using System.Text;
using AQI_Monitoring_System.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure MySQL database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Register services for dependency injection
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISensorService, SensorService>();

// Register AqiSimulationService as a singleton for injection
builder.Services.AddSingleton<AqiSimulationService>();

// Register AqiSimulationService as a hosted service (using the same singleton instance)
builder.Services.AddHostedService(provider => provider.GetRequiredService<AqiSimulationService>());
// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Home/Login";
        options.AccessDeniedPath = "/Home/Privacy";
    });

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var aqiSimulationService = app.Services.GetRequiredService<AqiSimulationService>();
aqiSimulationService.StartSimulation();

app.Run();