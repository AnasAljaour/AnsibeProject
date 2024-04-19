var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
/*builder.Services.AddDbContext<SchoolContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLserver"));
});
*/
builder.Services.AddControllersWithViews();


app.UseRouting();
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}"
    );

app.Run();
