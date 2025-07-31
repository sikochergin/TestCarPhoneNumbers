using Microsoft.EntityFrameworkCore;
using Data;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using TestCarPhoneNumbers.Services;

var builder = WebApplication.CreateBuilder(args);

// Чтение строки подключения из appsettings.json
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

// Регистрация DbContext
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services
    .AddScoped<INotificationService, DummyNotificationService>();



builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";   // куда редиректить неавторизованных
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        // можно настроить options.Cookie.Name, SlidingExpiration и т. д.
    });
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Стандартный маршрут
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();