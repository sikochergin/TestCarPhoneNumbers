using Microsoft.EntityFrameworkCore;
using Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// Чтение строки подключения из appsettings.json
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

// Регистрация DbContext
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Стандартный маршрут
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();