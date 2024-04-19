var builder = WebApplication.CreateBuilder(args);

/*builder.Services.AddDbContext<SchoolContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLserver"));
});
*/
builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseRouting();
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}"
    );

app.Run();
