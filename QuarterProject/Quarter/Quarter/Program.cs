using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quarter.DAL;
using Quarter.Models;
using Quarter.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddSignalR();


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
    Options.User.RequireUniqueEmail = true;
    Options.SignIn.RequireConfirmedEmail = true;

}).AddDefaultTokenProviders().AddEntityFrameworkStores<QuarterDbContext>();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = options.Events.OnRedirectToAccessDenied = context => 
    {
        if (context.HttpContext.Request.Path.Value.StartsWith("/admin") || context.HttpContext.Request.Path.Value.StartsWith("/Admin"))
        {
            var redirectPath = new Uri(context.RedirectUri);
            context.Response.Redirect("/admin/account/login" + redirectPath.Query);
        }
        else
        {
            var redirectPath = new Uri(context.RedirectUri);
            context.Response.Redirect("/account/login"+ redirectPath.Query);
        }
        return Task.CompletedTask;
    };
   

});



builder.Services.AddScoped<LayoutService>();

builder.Services.AddAuthentication()
               .AddGoogle(options =>
               {
                   options.ClientId = "575810266001-uej5f9or7hjuuab7c6lpa644ejlno7pr.apps.googleusercontent.com";
                   options.ClientSecret = "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";
                   options.SignInScheme = IdentityConstants.ExternalScheme;
               });



var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

  
}

//app.Use(async (context, next) =>
//{
//    await next();
//    if (context.Response.StatusCode == 404)
//    {
//        context.Response.Redirect("/home/error");
//    }
//});



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");





app.Run();
