using AnsibeProject.Data;
using AnsibeProject.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UniversityContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLserver"));
});

builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseRouting();
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}"
    );

app.Run();


