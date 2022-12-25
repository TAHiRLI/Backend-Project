using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quarter.DAL;
using Quarter.Models;
using Quarter.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<QuarterDbContext>(opt =>
{
    opt.UseSqlServer("Server=DESKTOP-6BCR9RQ; Database=Quarter; Trusted_Connection=TRUE ");
});


builder.Services.AddIdentity<AppUser, IdentityRole>(Options=>
{
    Options.Password.RequireDigit = false;
    Options.Password.RequireUppercase = false;
    Options.Password.RequiredLength = 8;
    Options.Password.RequireNonAlphanumeric = false;

}).AddDefaultTokenProviders().AddEntityFrameworkStores<QuarterDbContext>();

builder.Services.AddScoped<LayoutService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");





app.Run();
