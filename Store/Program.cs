using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Store.Services;
using Store.Models;
using Microsoft.Extensions.Configuration; // Make sure this is included

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MyStoreContext>(options =>  // Use MyStoreDataContext here
     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<MyStoreContext>();
builder.Services.AddControllersWithViews();

// Add the email service. Important: Add this *after* AddDbContext
builder.Services.AddScoped<IEmailService, EmailService>();

// Configure strongly typed settings. Important: Add this *after* AddDbContext
builder.Services.Configure<SmtpSettings>(options =>  // Corrected code
{
    builder.Configuration.GetSection("SmtpSettings").Bind(options);
});
builder.Services.Configure<EmailSettings>(options => // Corrected code
{
    builder.Configuration.GetSection("EmailSettings").Bind(options);
});

// Add logging (Log4Net example) - Adjust as needed
builder.Logging.AddLog4Net(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "products",  // Added a name for the products route
    pattern: "Products/{action=Index}/{id?}");  // More specific pattern
app.MapRazorPages();

app.Run();// Update the logging configuration to use the correct method signature

